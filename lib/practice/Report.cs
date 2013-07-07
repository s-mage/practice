using System.Collections.Generic;
namespace Rooletochka
{
	public struct ResultOfCheckPage
	{
		public bool InlineJS;
		public bool InlineCss;
		public bool TagHtml;
		public bool TagHead;
		public bool TagBody;
		public bool TagTitle;
		public string Url;
	}

	public class Report
	{
		private string _mainUrl;
		private bool _robotsTxtStatus;
		private bool _error404Status;
		private bool _redirectStatus;
		private List<ResultOfCheckPage> _listChildPages;
		public ResultOfCheckPage MainPageResult;

		public Report()
		{
			_mainUrl = "";
			_robotsTxtStatus = false;
			_error404Status = false;
			_redirectStatus = false;
			_listChildPages = new List<ResultOfCheckPage>();
		}

		public List<ResultOfCheckPage> ChildPagesResult {
			get { return _listChildPages; }
		}

		public bool RobotsTxt {
			get { return _robotsTxtStatus; }
			set { _robotsTxtStatus = value; }
		}
		public bool Error404 {
			get { return _error404Status; }
			set { _error404Status = value; }
		}
		public bool Redirect {
			get { return _redirectStatus; }
			set { _redirectStatus = value; }
		}
		public string MainUrl
		{
			get { return _mainUrl; }
			set { _mainUrl = value; }
		}

		public void AddResultOfChecking(ResultOfCheckPage result)
		{
			_listChildPages.Add(result);
		}

	}

}
