using System.Collections.Generic;
using Features = System.Collections.Generic.Dictionary<string, bool>;

namespace Rooletochka
{
	public class Report
	{
		// Names of common features.
		//
		private const string ROBOTS_TXT = "robotsTxt";
		private const string ERROR_404 = "error404";
		private const string REDIRECT = "redirect";

		private string mainUrl;

		// Common features are:
		//   robotsTxt, error404, redirect.
		//
		private Features commonFeatures;

		// Specific for each page features are:
		//  inlineJs, inlineCss, tagHtml, tagHead, tagBody, tagTitle, url.
		//
		private Dictionary<string, Features> specificFeatures;

		// Features for main page.
		//
		public Features mainPageResult;

		public Report()
		{
			mainUrl = "";
			commonFeatures = new Dictionary<string, bool>();
			specificFeatures = new Dictionary<string, Features>();
			mainPageResult = new Dictionary<string, bool>();
		}

		public Dictionary<string, Features> SpecificFeatures {
			get { return specificFeatures; }
		}

		public bool RobotsTxt {
			get { return commonFeatures[ROBOTS_TXT]; }
			set { commonFeatures[ROBOTS_TXT] = value; }
		}
		public bool Error404 {
			get { return commonFeatures[ERROR_404]; }
			set { commonFeatures[ERROR_404] = value; }
		}
		public bool Redirect {
			get { return commonFeatures[REDIRECT]; }
			set { commonFeatures[REDIRECT] = value; }
		}
		public string MainUrl
		{
			get { return mainUrl; }
			set { mainUrl = value; }
		}

		public void AddCheckedPage(Features result, string url)
		{
			specificFeatures.Add(url, result);
		}
	}
}
