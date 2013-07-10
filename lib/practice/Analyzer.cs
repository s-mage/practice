using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Rooletochka {
	public class Analyzer {
		private const byte MAX_CHILD_PAGE_IN_REPORT = 15;
		private readonly string _url;
		private readonly string _content;
		private List<String> _pages;

		public string Url {
			get { return _url; }
		}

		public string Content {
			get { return _content; }
		}

		public Analyzer(string url, bool isMainPage) {
			_url = url.ToLower();

			if (IsCorrectURL(_url)) {
				WebClient client = new WebClient();
				_content = client.DownloadString(_url).ToLower();

				if (isMainPage) _pages = GetPages(_content, _url);
				else _pages = new List<string>();
			}
		}

		public Report Analyze() {
			Report report = new Report();
			report.MainUrl = Url;
			report.RobotsTxt = CheckRobotsTxt(Url);
			Thread.Sleep(500);
			report.Error404 = CheckError404(Url);

			ResultOfCheckPage result = new ResultOfCheckPage();
			report.MainPageResult = this.AnalyzePage(Url);

			foreach (string page in _pages) {
				try {
					Analyzer analyzer = new Analyzer(page, false);
					result = analyzer.AnalyzePage(analyzer.Url);
					report.AddCheckedPage(result);
					if (report.ChildPagesResult.Count == MAX_CHILD_PAGE_IN_REPORT) break;
					Thread.Sleep(500);
				}
				catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			}
			return report;
		}

		private bool IsCorrectURL(string url) {
			Uri correctUrl;
			if (Uri.TryCreate(url, UriKind.Absolute, out correctUrl) &&
			    correctUrl.Scheme == Uri.UriSchemeHttp) {
				return true;
			}
			return false;
		}

		// check on correct url and than link != link to the file, except .php
		private bool IsCorrectLink(string link) {
			if (!IsCorrectURL(link)) return false;

			int count = link.Length - 1;
			string buffer = "";
			while (count >= 0) {
				buffer = link[count] + buffer;
				count--;
				if (link[count] == '.' || link[count] == '/')
					break;
			}
			if (buffer.Length <= 3) {
				if (buffer.ToLower().Contains("php"))
					return true;
				return false;
			}
			return true;
		}

		private ResultOfCheckPage AnalyzePage(string url) {
			ResultOfCheckPage result = new ResultOfCheckPage
				{
					Url = url,
					TagBody = CheckBodyTag(Content),
					TagHead = CheckHeadTag(Content),
					TagTitle = CheckTitleTags(Content),
					TagHtml = CheckHtmlTag(Content),
					InlineJS = CheckInlineJS(Content),
					InlineCss = CheckInlineCSS(Content)
				};
			return result;
		}

		private List<String> GetPages(string content, string url) {
			url = url.TrimEnd('/');
			List<String> pages = new List<String>();
			string pattern = @"<a.*?href\s*=(['""][^""]*['""])";
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(content);
			foreach (Match match in matches) {
				string link = Regex.Replace(match.ToString(),
				                            @"<a.*?href\s*=(['""][^""]*['""])", @"$1",
				                            RegexOptions.IgnoreCase);
				link = link.Trim("\"".ToCharArray());
				if (link.Length > 2 && (link[0] == '/' || link[0] == '.' || link.Contains(url))) {
					if (link[0] == '/' && link[1] == '/')
						continue;
					if ((link[0] == '/') || (link[0] == '.' && link[1] == '/'))
						link = url + link;
					if (link.Contains(url)) {}
					else continue;
				}
				else continue;

				if (IsCorrectLink(link)) {
					pages.Add(link);
				}
			}
			return pages;
		}

		#region Methods for checking common rules

		public bool CheckRobotsTxt(string url) {
			string str = "";
			if (url[url.Length - 1] == '/') str = url + "robots.txt";
			else str = url + "/robots.txt";

			const bool redirect = false;
			int statusCode = CheckStatusCode(str, redirect);
			if (statusCode > 400 || statusCode == 0) return false;
			return true;
		}

		public bool CheckError404(string url) {
			string str = "";
			if (url[url.Length - 1] == '/') str = url + "asdfjhkxjcv";
			else str = url + "/asdfjhkxjcv";

			bool redirect = true;
			int statusCode = CheckStatusCode(str, redirect);
			return (statusCode == 404);
		}

		public bool CheckMirror(string url) {
			//TODO функцию генерирующую список (массив) url'ов для зеркал сайта
			bool redirect = false;
			CheckStatusCode(url, redirect);
			return false;
		}

		private int CheckStatusCode(string url, bool redirect) {
			try {
				HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);
				webRequest.AllowAutoRedirect = redirect;
				//Timeout of request (default timeout = 100s)
				webRequest.Timeout = 50000;
				HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse();
				int wRespStatusCode;
				wRespStatusCode = (int) response.StatusCode;
				return wRespStatusCode;
			}
			catch (WebException we) {
				try {
					int wRespStatusCode = (int) ((HttpWebResponse) we.Response).StatusCode;
					return wRespStatusCode;
				}
				catch (NullReferenceException e) {
					Console.WriteLine(e.Message);
					return 0;
				}
			}
		}

		#endregion

		#region Methods for checking Html tags (true - OK, false - necessary corrections)

		private bool CheckInlineJS(string content) {
			string pattern = @"<script.*?>";
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(content);
			foreach (Match match in matches) {
				string value = match.ToString();
				if (value.Contains("src") && value.Contains(".js")) continue;
				return false;
			}
			return true;
		}

		private bool CheckInlineCSS(string content) {
			string pattern = @"style\s*=\s*"".*?""";
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(content);
			if (matches.Count == 0) return true;
			return false;
		}

		private bool CheckTitleTags() {
			for (int i=0; i<6; i++) {
				if (CheckTag("h"+i)) { return true; }
			}
			return false;
		}

		private bool CheckHtmlTag() {
			return CheckTag("html");
		}

		private bool CheckBodyTag() {
			return CheckTag("body");
		}

		private bool CheckHeadTag() {
			return CheckTag("head");
		}

		// Check page for opening and closing tags.
		// Example: CheckTag("html") will check page for <html> and </html>.
		//
		private bool CheckTag(string tag) {
			return content.Contains("<"+tag)&&content.Contains("</"+tag+">");
		}

		#endregion
	}
}