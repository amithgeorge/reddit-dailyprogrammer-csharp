using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Xunit;

namespace RedditDailyProgrammer.Answers._207Bonus
{
    public class ErdosNumberCalculatorTests
    {
        private const string Erdos = "Erdös, P.";
        private const string Schelp = "Schelp, R. H.";
        private const string Burris = "Burris, A. C.";
        private const string Schwenk = "Schwenk, A. J.";

        [Fact]
        public void Erdos_has_number_0()
        {
            var results = new ErdosNumberCalculator().Process(GetSampleData());

            Assert.Equal(0, results[Erdos]);
        }

        [Fact]
        public void Erdos_collaborators_have_number_1()
        {
            var results = new ErdosNumberCalculator().Process(GetSampleData());

            Assert.Equal(1, results[Schelp]);
        }

        [Fact]
        public void Collaborators_one_relation_away_have_number_2()
        {
            var results = new ErdosNumberCalculator().Process(GetSampleData());

            Assert.Equal(2, results[Burris]);
        }

        [Fact]
        public void Two_collaborators_with_an_existing_relation_to_Erdos_retain_their_numbers()
        {
            var data = GetSampleData();
            Assert.True(data.Any(x => x.Authors.Contains(Erdos) && x.Authors.Contains(Schwenk)),
                        "Improper sample data");
            Assert.True(data.Any(x => x.Authors.Contains(Erdos) && x.Authors.Contains(Schelp)),
                        "Improper sample data");
            Assert.True(data.Any(x => x.Authors.Contains(Schwenk) && x.Authors.Contains(Schelp)),
                        "Improper sample data");

            var results = new ErdosNumberCalculator().Process(data);

            Assert.Equal(1, results[Schelp]);
            Assert.Equal(1, results[Schwenk]);
        }

        [Fact]
        public void Can_create_graph_from_collaboration_data()
        {
            var graph = new ErdosNumberCalculatorUT().CreateGraph_Ex(GetSampleData());

            Assert.Equal(8, graph[Erdos].Count);
            Assert.False(graph[Erdos].Contains(Erdos));
            Assert.True(graph[Schelp].Contains(Schwenk));
            Assert.True(graph[Schelp].Contains(Erdos));
            Assert.True(graph[Schwenk].Contains(Erdos));
            Assert.True(graph[Schwenk].Contains(Schelp));
            Assert.True(graph[Burris].Contains(Schwenk));
            Assert.True(graph[Burris].Contains(Schelp));
        }

        [Fact]
        public void Can_calculates_distances_from_Erdos()
        {
            var graph = new Dictionary<string, HashSet<string>>();
            graph[Erdos] = new HashSet<string>() { "B", "C" };
            graph["B"] = new HashSet<string>() { Erdos, "D" };
            graph["C"] = new HashSet<string>() { Erdos };
            graph["D"] = new HashSet<string>() { "B", "E" };
            graph["E"] = new HashSet<string>() { "D" };

            var distances = new ErdosNumberCalculatorUT().GetDistances_Ex(graph);

            Assert.Equal(0, distances[Erdos]);
            Assert.Equal(1, distances["B"]);
            Assert.Equal(1, distances["C"]);
            Assert.Equal(2, distances["D"]);
            Assert.Equal(3, distances["E"]);
        }

//        [Fact]
//        public void Can_find_shortest_path_in_graph()
//        {
//            var graph = new Dictionary<string, HashSet<string>>();
//            graph["Erdos"] = new HashSet<string>(){"B", "C"};
//            graph["B"] = new HashSet<string>(){"Erdos", "D"};
//            graph["C"] = new HashSet<string>(){"Erdos"};
//            graph["D"] = new HashSet<string>(){"B", "E"};
//            graph["E"] = new HashSet<string>(){"D"};
//
//            Assert.Equal(0, new ErdosNumberCalculatorUT().ShortestPath_Ex("Erdos", graph));
//            Assert.Equal(1, new ErdosNumberCalculatorUT().ShortestPath_Ex("B", graph));
//            Assert.Equal(1, new ErdosNumberCalculatorUT().ShortestPath_Ex("C", graph));
//            Assert.Equal(2, new ErdosNumberCalculatorUT().ShortestPath_Ex("D", graph));
//            Assert.Equal(3, new ErdosNumberCalculatorUT().ShortestPath_Ex("E", graph));
//        }


        private List<CollarboratorData> GetSampleData()
        {
            return new List<CollarboratorData>()
                   {
                       new CollarboratorData()
                       {
                           Title = "Vertex-distinguishing proper edge-colorings",
                           Authors = new List<string>()
                                     {
                                         Burris,
                                         Schelp,
                                         Schwenk
                                     }
                       },
                       new CollarboratorData()
                       {
                           Title = "Tight bounds on the chromatic sum of a connected graph",
                           Authors = new List<string>()
                                     {
                                         "Thomassen, C.",
                                         Erdos,
                                         "Alavi, Y.",
                                         "Malde, P. J.",
                                         Schwenk
                                     }
                       },
                       new CollarboratorData()
                       {
                           Title = "Some complete bipartite graph—tree Ramsey numbers",
                           Authors = new List<string>()
                                     {
                                         "Burr, S.",
                                         Erdos,
                                         "Faudree, R. J.",
                                         "Rousseau, C. C.",
                                         Schelp
                                     }
                       }
                   };
        } 
    }

    public class InputParserTests
    {
        private const string Input = @"3 2
Thomassen, C., Erdös, P., Alavi, Y., Malde, P. J., & Schwenk, A. J. (1989). Tight bounds on the chromatic sum of a connected graph. Journal of Graph Theory, 13(3), 353-357.
Burr, S., Erdös, P., Faudree, R. J., Rousseau, C. C., & Schelp, R. H. (1988). Some complete bipartite graph—tree Ramsey numbers. Annals of Discrete Mathematics, 41, 79-89.
Balister, P. N., Riordan, O. M., & Schelp, R. H. (2003). Vertex‐distinguishing edge colorings of graphs. Journal of graph theory, 42(2), 95-109.
Schelp, R. H.
Burris, A. C.
";

        [Fact]
        public void Can_detect_correct_num_of_lines_as_mentioned_in_line1_of_input()
        {
            var parser = new InputParser();
            
            var result = parser.Parse(Input);

            Assert.Equal(3, result.Collaborations.Count);
            Assert.Equal(2, result.AuthorsToQuery.Count);
        }

        [Fact]
        public void Can_parse_a_book_line()
        {
            var parser = new InputParser();

            var result = parser.Parse(Input);

            Assert.Equal("Tight bounds on the chromatic sum of a connected graph.", result.Collaborations.First().Title);
            Assert.True(result.Collaborations.First().Authors.Any(a => a.Equals("Erdös, P.")));
            Assert.True(result.Collaborations.First().Authors.Count == 5);
        }

        [Fact]
        public void Throws_exception_if_failed_parse_book_line()
        {
            var parser = new InputParserUT();
            const string line = "Thomassen, C., Erdös, P., Alavi, Y., Malde, P. J., & Schwenk, A. J." +
                                "Tight bounds on the chromatic sum of a connected graph";
            Assert.Throws<ArgumentException>(() =>
                                             {
                                                 parser.ParseBookLine_Ex(line);
                                             });
        }

        [Fact]
        public void Can_parse_author_lines()
        {
            var parser = new InputParser();

            var result = parser.Parse(Input);

            Assert.Equal(2, result.AuthorsToQuery.Count);
        }

        [Fact]
        public void Can_parse_all_book_lines()
        {
            var parser = new InputParser();

            var result = parser.Parse(Input);

            Assert.Equal(3, result.Collaborations.Count);
        }
    }

    public class Main
    {
        [Fact]
        public void Run()
        {
            const string input = @"7 4
Thomassen, C., Erdös, P., Alavi, Y., Malde, P. J., & Schwenk, A. J. (1989). Tight bounds on the chromatic sum of a connected graph. Journal of Graph Theory, 13(3), 353-357.
Burr, S., Erdös, P., Faudree, R. J., Rousseau, C. C., & Schelp, R. H. (1988). Some complete bipartite graph—tree Ramsey numbers. Annals of Discrete Mathematics, 41, 79-89.
Burris, A. C., & Schelp, R. H. (1997). Vertex-distinguishing proper edge-colorings. Journal of graph theory, 26(2), 73-82.
Balister, P. N., Gyo˝ ri, E., Lehel, J., & Schelp, R. H. (2007). Adjacent vertex distinguishing edge-colorings. SIAM Journal on Discrete Mathematics, 21(1), 237-250.
Erdös, P., & Tenenbaum, G. (1989). Sur les fonctions arithmétiques liées aux diviseurs consécutifs. Journal of Number Theory, 31(3), 285-311.
Hildebrand, A., & Tenenbaum, G. (1993). Integers without large prime factors. Journal de théorie des nombres de Bordeaux, 5(2), 411-484.
Balister, P. N., Riordan, O. M., & Schelp, R. H. (2003). Vertex‐distinguishing edge colorings of graphs. Journal of graph theory, 42(2), 95-109.
Schelp, R. H.
Burris, A. C.
Riordan, O. M.
Balister, P. N.
";
            var inputData = new InputParser().Parse(input);
            var erdosValues = new ErdosNumberCalculator().Process(inputData.Collaborations);

            const string expectedOutput = @"Schelp, R. H. 1
Burris, A. C. 2
Riordan, O. M. 2
Balister, P. N. 2
";

            var builder = new StringBuilder();
            inputData.AuthorsToQuery
                     .ForEach(a =>
                              {
                                  var line = string.Format("{0} {1}", a, erdosValues[a]);
//                                  Console.WriteLine(line);
                                  builder.AppendLine(line);
                              });

            Assert.Equal(expectedOutput, builder.ToString());
            

            Assert.Equal(1, erdosValues["Schelp, R. H."]);
            Assert.Equal(2, erdosValues["Burris, A. C."]);
            Assert.Equal(2, erdosValues["Riordan, O. M."]);
            Assert.Equal(2, erdosValues["Balister, P. N."]);
        }
    }

// ReSharper disable once InconsistentNaming
    public class ErdosNumberCalculatorUT : ErdosNumberCalculator
    {
        public Dictionary<string, HashSet<string>> CreateGraph_Ex(List<CollarboratorData> data)
        {
            return CreateGraph(data);
        }

        public Dictionary<string, int> GetDistances_Ex(Dictionary<string, HashSet<string>> graph)
        {
            return GetDistances(graph);
        }
    }

// ReSharper disable once InconsistentNaming
    public class InputParserUT : InputParser
    {
        public CollarboratorData ParseBookLine_Ex(string line)
        {
            return ParseBookLine(line);
        }
    }
}
