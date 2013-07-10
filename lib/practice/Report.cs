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

		// Features for main page.
		//
		public Features mainPageResult;

		// Common features are:
		//   robotsTxt, error404, redirect.
		//
		private Features commonFeatures;

		// Specific for each page features are:
		//  inlineJs, inlineCss, tagHtml, tagHead, tagBody, tagTitle, url.
		//
		private List<Page> specificFeatures;

		// Class for store features of page. Consists of URL of page
		// and it's features.
		public class Page
		{
			private string url;
			private Features features;

			public Page(string uri, Features f)
			{
				url = uri;
				features = f;
			}

			public string URL {
				get { return url; }
			}

			public Features Features {
				get { return features; }
			}
		}

		public Report()
		{
			mainUrl = "";
			commonFeatures = new Dictionary<string, bool>();
			specificFeatures = new List<Page>();
			mainPageResult = new Dictionary<string, bool>();
		}

		public List<Page> SpecificFeatures {
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

		public void AddCheckedPage(Features features, string url)
		{
			Page page = new Page(url, features);
			specificFeatures.Add(page);
		}
	}
}
