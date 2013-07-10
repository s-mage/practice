using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Features = System.Collections.Generic.Dictionary<string, bool>;

namespace Rooletochka
{
	public class Analyzer
	{
		// Count of analyzed pages.
		//
		private const int COUNT_OF_PAGES = 10;

		// Names of features for each page on site.
		//
		private const string TAG_BODY = "tagBody";
		private const string TAG_HTML = "tagHtml";
		private const string TAG_HEAD = "tagHead";
		private const string TAG_TITLE = "tagTitle";
		private const string INLINE_JS = "inlineJs";
		private const string INLINE_CSS = "inlineCss";

		private readonly string url;
		private string content;
		private List<String> pages;

		public string Url
		{
			get { return url; }
		}

		public string Content
		{
			get { return content; }
		}

		public Analyzer(string uri, bool mainPage)
		{
			url = uri.ToLower();

			WebClient client = new WebClient();
			content = client.DownloadString(url).ToLower();

			if(mainPage) { pages = GetPages(content, url); }
			else { pages = new List<string>(); }
		}

		public Report Analyze()
		{
			Report report = new Report();
			report.MainUrl = Url;
			report.RobotsTxt = CheckRobotsTxt(Url);
			Thread.Sleep(500);
			report.Error404 = CheckError404(Url);

			report.mainPageResult = AnalyzePage(Url);

			Features result = new Features();
			foreach (string page in pages)
			{
				try
				{
					Analyzer analyzer = new Analyzer(page, false);
					result = analyzer.AnalyzePage(analyzer.Url);
					report.AddCheckedPage(result, page);
					Thread.Sleep(500);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			return report;
		}

		// Analyze one page of site.
		//
		private Features AnalyzePage(string url)
		{
			Features result = new Features();
			result[TAG_BODY] = CheckBodyTag();
			result[TAG_HEAD] = CheckHeadTag();
			result[TAG_TITLE] = CheckTitleTags();
			result[TAG_HTML] = CheckHtmlTag();
			result[INLINE_JS] = CheckInlineJS();
			result[INLINE_CSS] = CheckInlineCSS();
			return result;
		}

		private List<String> GetPages(string content, string url)
		{
			url = url.TrimEnd('/');
			List<String> pages = new List<String>();
			string pattern = @"<a.*?href\s*=(['""][^""]*['""])";
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(content);

			foreach (Match match in matches)
			{
				string link = Regex.Replace(match.ToString(),
					@"<a.*?href\s*=(['""][^""]*['""])", @"$1", RegexOptions.IgnoreCase);
				link = link.Trim("\"".ToCharArray());
				if (link.Length > 2 && (link[0] == '/' || link.Contains(url)))
				{
					if (link[0] == '/' && link[1] == '/') continue;
					if (link[0] == '/') link = url + link;
					pages.Add(link);

					if(pages.Count == COUNT_OF_PAGES) { return pages; }
				}
			}

			return pages;
		}

		#region Methods for checking common rules
		public bool CheckRobotsTxt(string url)
		{
			string str = "";
			if (url[url.Length - 1] == '/') str = url + "robots.txt";
			else str = url + "/robots.txt";

			bool redirect = false;
			int statusCode = CheckStatusCode(str, redirect);
			if (statusCode > 400 || statusCode == 0) return false;
			else return true;
		}

		public bool CheckError404(string url)
		{
			string str = "";
			if (url[url.Length - 1] == '/') str = url + "asdfjhkxjcv";
			else str = url + "/asdfjhkxjcv";

			bool redirect = true;
			int statusCode = CheckStatusCode(str, redirect);
			return (statusCode == 404);
		}

		private int CheckStatusCode(string url, bool redirect)
		{
			try
			{
				HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);
				webRequest.AllowAutoRedirect = redirect;
				//Timeout of request (default timeout = 100s)
				webRequest.Timeout = 50000;
				HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse();
				int wRespStatusCode;
				wRespStatusCode = (int) response.StatusCode;
				return wRespStatusCode;
			}
			catch (WebException we)
			{
				try
				{
					int wRespStatusCode = (int) ((HttpWebResponse) we.Response).StatusCode;
					return wRespStatusCode;
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(e.Message);
					return 0;
				}
			}
		}

		#endregion

		#region Methods for checking Html tags (true - OK, false - page needs corrections)
		private bool CheckInlineJS()
		{
			string pattern = @"<script.*?>";
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(content);
			foreach (Match match in matches)
			{
				string value = match.ToString();
				if (value.Contains("src") && value.Contains(".js")) continue;
				return false;
			}
			return true;
		}

		private bool CheckInlineCSS()
		{
			string pattern = @"style\s*=\s*"".*?""";
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(content);
			if (matches.Count == 0) return true;
			return false;
		}

		private bool CheckTitleTags()
		{
			for (int i = 0; i < 6; i++) {
				if (CheckTag("h" + i)) { return true; }
			}
			return false;
		}

		private bool CheckHtmlTag()
		{
			return CheckTag("html");
		}

		private bool CheckBodyTag()
		{
			return CheckTag("body");
		}

		private bool CheckHeadTag()
		{
			return CheckTag("head");
		}

		// Check page for opening and closing tags.
		// Example: CheckTag("html") will check page for <html> and </html>.
		//
		private bool CheckTag(string tag)
		{
			return content.Contains("<" + tag) && content.Contains("</" + tag + ">");
		}

		#endregion
	}
}
