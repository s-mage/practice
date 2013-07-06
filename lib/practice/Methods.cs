using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace testApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string url = "http://d3.ru/";
            url = url.ToLower();
            
            // Get site content
            //First method (faster)
            WebClient client = new WebClient();
            string content = client.DownloadString(url);
            content = content.ToLower();
            //Console.WriteLine(content);
            
            //Second method
            //string content = GetHtmlPageText(url);
            
            List<String> list = GetLinksFromContentSite(content, url);
            foreach (string elem in list)
            {
                Console.WriteLine(elem);
            }
            Thread.Sleep(500);
            Console.WriteLine(CheckStatusCode(url, false));
            Thread.Sleep(500);
            Console.WriteLine("Error 404 " + CheckError404(url));
            Thread.Sleep(500);
            
            Console.WriteLine("Titles " + CheckTitleTags(content));
            Console.WriteLine("HtmlTag " + CheckHtmlTag(content));
            Console.WriteLine("BodyTag " + CheckBodyTag(content));
            Console.WriteLine("Robots.txt " + CheckRobotsTxt(url));
            Console.ReadLine();
        }



        public static string GetHtmlPageText(string url) {
            string txt = String.Empty;
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    txt = sr.ReadToEnd();
                }
            }
            return txt.ToLower();
        }

        public static List<String> GetLinksFromContentSite(string content, string url)
        {
            url = url.TrimEnd('/');
            List <String> listOfLinks= new List<String>();
            //string pattern = @"<a.*?href\s*=(""" + url + @"[^""]*"")";
            string pattern = @"<a.*?href\s*=(['""][^""]*['""])";
            Regex rgx = new Regex(pattern);
            MatchCollection matches = rgx.Matches(content);
            foreach (Match match in matches)
            {
                string link = Regex.Replace(match.ToString(), @"<a.*?href\s*=(['""][^""]*['""])", @"$1", RegexOptions.IgnoreCase);
                link = link.Trim("\"".ToCharArray());
                if (link.Length > 2 && (link[0] == '/' || link.Contains(url)))
                {
                    if ((link[0] == '/' && link[1] == '/') == true) continue;
                    if (link[0] == '/') link = url + link;
                    listOfLinks.Add(link);
                }
            }
            return listOfLinks;
        }

        public static bool CheckTitleTags(string content)
        {
            string titleTag = "<h";
            string closingTitleTag = "</h";
            for (int i = 0; i < 6; i++)
            {
                if ((content.Contains(titleTag + i) && content.Contains(closingTitleTag + i)) == true)
                    return true;
            }
            return false;
        }   

        public static bool CheckRobotsTxt(string url)
        {
            string str = "";
            if (url[url.Length - 1] == '/') str = url + "robots.txt";
            else str = url + "/robots.txt";

            bool redirect = false;
            int statusCode = CheckStatusCode(str, redirect);
            if (statusCode > 400 || statusCode == 0) return false;
            else return true;
        }


        public static bool CheckError404(string url)
        {
            string str = "";
            if (url[url.Length - 1] == '/') str = url + "asdfjhkxjcv";
            else str = url + "/asdfjhkxjcv";
            
            bool redirect = true;
            int statusCode = CheckStatusCode(str, redirect);
            if (statusCode == 404) return true;
            else return false;
        }

        public static bool CheckHtmlTag(string content)
        {
            string openHeadTag = "<html";
            string closingHeadTag = "</html>";
            if ((content.Contains(openHeadTag) && content.Contains(closingHeadTag)) == true)
                return true;
            else return false;
        }

        public static bool CheckBodyTag(string content)
        {
            string openBodyTag = "<body";
            string closingBodyTag = "</body>";
            if ((content.Contains(openBodyTag) && content.Contains(closingBodyTag)) == true)
                return true;
            else return false;
        }

        private static int CheckStatusCode(string url, bool redirect)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.AllowAutoRedirect = redirect;
                //Timeout of request (default timeout = 100s)
                webRequest.Timeout = 50000; 
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                int wRespStatusCode;
                wRespStatusCode = (int)response.StatusCode;
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


        //unused (more slowly than string.Contains)
        public static bool ContainsString(string text, string str)
        {
            if (str.Length > text.Length || str == "" || text == "")
                return false;

            StringBuilder buffer = new StringBuilder();
            int position = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == str[position])
                {
                    buffer.Append(str[position]);
                    position++;
                    if (buffer.ToString() == str) return true;
                }
                else
                {
                    buffer.Clear();
                    position = 0;
                }
            }
            return false;
        }
    }
}
