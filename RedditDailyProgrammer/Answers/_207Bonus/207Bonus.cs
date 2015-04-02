using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditDailyProgrammer.Answers._207Bonus
{
    public class ErdosNumberCalculator
    {
        private const string Erdos = "Erdös, P.";

        public IReadOnlyDictionary<string, int> Process(List<CollarboratorData> data)
        {
            return GetDistances(CreateGraph(data));
        }

        protected Dictionary<string, HashSet<string>> CreateGraph(List<CollarboratorData> data)
        {
            var graph =
                data.SelectMany(x => x.Authors,
                                (x, a) => x.Authors.Select(b => new {From = a, To = b})
                                           .Where(p => p.From != p.To)
                                           .ToList())
                    .SelectMany(x => x)
                    .Aggregate(new Dictionary<string, HashSet<string>>(),
                               (acc, edge) =>
                               {
                                   if (acc.ContainsKey(edge.From) == false)
                                   {
                                       acc[edge.From] = new HashSet<string>();
                                   }
                                   acc[edge.From].Add(edge.To);
                                   return acc;
                               });

            return graph;
        }

        protected Dictionary<string, int> GetDistances(Dictionary<string, HashSet<string>> graph)
        {
            var q = new Queue<string>();
            var distances = new Dictionary<string, int>();

            q.Enqueue(Erdos);
            distances[Erdos] = 0;

            while (q.Any())
            {
                var current = q.Dequeue();
                graph[current]
                    .Where(x => distances.ContainsKey(x) == false)
                    .ToList()
                    .ForEach(x =>
                             {
                                 q.Enqueue(x);
                                 distances[x] = distances[current] + 1;
                             });
            }

            return distances;
        }
    }

    public class InputParser
    {
        private const string BookLineRegex = @"(?<authors>.*)\(\d{4}\)\.(?<title>.*?\.)";

        public InputData Parse(string input)
        {
            var inputData = new InputData();

            using (var reader = new StringReader(input))
            {
                // ReSharper disable once PossibleNullReferenceException
                var line1Parts = reader.ReadLine().Split(new[] { ' ' });
                var bookLinesCount = Int32.Parse(line1Parts[0]);
                var authorLinesCount = Int32.Parse(line1Parts[1]);

                inputData.Collaborations =
                    Enumerable.Range(0, bookLinesCount)
                          .Select(_ =>
                          {
                              try
                              {
                                  return ParseBookLine(reader.ReadLine());
                              }
                              catch (Exception e)
                              {
                                  e.Data["Message"] = "Error parsing book line";
                                  e.Data["CurrentBookLine"] = _;
                                  e.Data["NumBookLines"] = bookLinesCount;
                                  throw;
                              }
                          })
                          .ToList();

                try
                {
                    inputData.AuthorsToQuery =
                        Enumerable.Range(0, authorLinesCount)
                                  .Select(_ => reader.ReadLine())
                                  .ToList();
                }
                catch (Exception e)
                {
                    e.Data["Message"] = "Error parsing author lines";
                    e.Data["NumAuthorLines"] = authorLinesCount;
                    throw;
                }
            }

            return inputData;
        }

        protected CollarboratorData ParseBookLine(string line)
        {
            // Thomassen, C., Erdös, P., Alavi, Y., Malde, P. J., & Schwenk, A. J. (1989). Tight bounds on the chromatic sum of a connected graph. Journal of Graph Theory, 13(3), 353-357.
            var match = new Regex(BookLineRegex).Match(line);
            if (match.Success == false ||
                match.Groups["title"].Success == false ||
                match.Groups["authors"].Success == false)
            {
                var exception = new ArgumentException("Cannot parse book line");
                exception.Data["Line"] = line;
                throw exception;
            }

            var data =
                new CollarboratorData
                {
                    Title = match.Groups["title"].Value.Trim(),
                    Authors = match.Groups["authors"]
                        .Value
                        // felt this was easier than splitting by comma and then partioning & joining the lastname,restname pairs
                        .Split(new[] { "., " }, StringSplitOptions.None)
                        .Select(a => a.TrimStart('&'))
                        .Select(a => a.Trim())
                        // all names except the last need to have a period appended
                        .Select(a => a.EndsWith(".") ? a : a + ".")
                        .ToList()
                };

            return data;
        }
    }

    public class CollarboratorData
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }

        public CollarboratorData()
        {
            Authors = new List<string>();
        }
    }
    
    public class InputData
    {
        public List<CollarboratorData> Collaborations { get; set; }
        public List<string> AuthorsToQuery { get; set; }

        public InputData()
        {
            Collaborations = new List<CollarboratorData>();
            AuthorsToQuery = new List<string>();
        }
    }

}
