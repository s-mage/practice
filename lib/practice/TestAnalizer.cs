using System;
using System.Collections.Generic;

namespace testApplication
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string url = "http://d3.ru/";
			//TODO get url from bd
			
			Analyzer analyzer = new Analyzer(url, true);
			Report report = new Report();
			report = analyzer.StartAnalysis();
			Console.WriteLine("Error404 " + report.Error404);
			Console.WriteLine("Robots " + report.RobotsTxt);
			List<ResultOfCheckPage> list = report.ChildPagesResult;
			Console.WriteLine("Count child page " + list.Count);
			Console.Read();
		}
	}
}
