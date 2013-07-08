using System.Collections.Generic;
using Features = System.Collections.Generic.Dictionary<string, bool>;

namespace Rooletochka
{
	public class Report
	{
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
			specificFeatures = new Dictionary<string, Dictionary<string, bool>>();
			mainPageResult = new Dictionary<string, bool>();
		}

		public Dictionary<string, Features> SpecificFeatures {
			get { return specificFeatures; }
		}

		public bool RobotsTxt {
			get { return commonFeatures["robotsTxt"]; }
			set { commonFeatures["robotsTxt"] = value; }
		}
		public bool Error404 {
			get { return commonFeatures["error404"]; }
			set { commonFeatures["error404"] = value; }
		}
		public bool Redirect {
			get { return commonFeatures["redirect"]; }
			set { commonFeatures["redirect"] = value; }
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
