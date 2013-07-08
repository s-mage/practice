using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace Rooletochka
{
	internal class Program
	{
		public Model CreateModel()
		{
			string connect = "Server=127.0.0.1;Port=5432;User Id=s;Database=practice;";
      var connection = new NpgsqlConnection(connect);
      return new Model(connection);
		}
		private static void Main(string[] args)
		{
			//var model = CreateModel();
			//NpgsqlDataReader urlRow = model.GetUrl();

			//string url = urlRow.GetString(2);

			string url = "http://d3.ru/";
			//TODO get url from bd

			Analyzer analyzer = new Analyzer(url, true);
			Report report = new Report();
			report = analyzer.Analyze();
			Console.WriteLine("Error404 " + report.Error404);
			Console.WriteLine("Robots " + report.RobotsTxt);
			//List<ResultOfCheckPage> list = report.ChildPagesResult;
			//Console.WriteLine("Count child page " + list.Count);
			Console.Read();
		}
	}
}
