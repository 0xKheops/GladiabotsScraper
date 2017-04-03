using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace GladiabotsScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var playerName = "Ritter Runkel";
            var url = String.Format("http://gfx47.com/games/Gladiabots/Stats/player?version=790&name={0}&display=matches", HttpUtility.UrlEncode(playerName));

            var task = GetResponseText(url);
            task.Wait();

            var regMatch = new Regex(@"<div class=""match"">\[(.*?)\].*?value=""(.*?)"".*?class=""result (.*?)"".*?<a.*?>(.*?)</a>.*?on (.*?)(</b>)?</div>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var allMatches = regMatch.Matches(task.Result).Cast<Match>().Select(m => new MatchInfo(){
                                                                                        Date = DateTime.Parse(m.Groups[1].Value),
                                                                                        Player = playerName,
                                                                                        Opponent = m.Groups[4].Value,
                                                                                        ReplayId = Int32.Parse(m.Groups[2].Value),
                                                                                        Result = m.Groups[3].Value,
                                                                                        MapName = m.Groups[5].Value
                                                                                    }).ToArray();

            if (allMatches.Length == 0) return;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(1033);

            var fileNameCsv = String.Format("{0}-{1}.csv", playerName, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            WriteCSV(allMatches, fileNameCsv);
        }

        public static async Task<String> GetResponseText(String address)
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync(address);
        }

        public static void WriteCSV<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
            }
        }

    }
}
