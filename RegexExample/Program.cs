using System;
using System.Text.RegularExpressions;

namespace RegexExample
{
    class Program
    {
        static void Main(string[] args)
        {
            MatchesGroupsCaptures();
        }

        private static void MatchesGroupsCaptures()
        {
            string pattern = @"(\w+)\s+(car)";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

            string input = "One car red car blue car big car";
            Match match = reg.Match(input);

            Console.WriteLine("###########################");
            Console.WriteLine("===========================");

            int matchCount = 0;
            while (match.Success)
            {
                Console.WriteLine("Match" + (++matchCount));
                Console.WriteLine(match.Value);

                var captures = match.Captures;
                Console.WriteLine("Captures:");
                foreach (Capture capture in captures)
                    Console.WriteLine("\t" + "Value: " + capture.Value +
                                "\n" + "\t" + "Index: " + capture.Index);

                var groups = match.Groups;
                Console.WriteLine("Groups:");
                foreach (Group group in groups)
                {
                    Console.WriteLine("\t" + "Value: " + group.Value +
                               "\n" + "\t" + "Index: " + group.Index);
                    Console.WriteLine();
                }

                Console.WriteLine("================");
                Console.WriteLine();

                match = match.NextMatch();
            }

            Console.WriteLine("===========================");
            Console.WriteLine("###########################");
        }
    }
}
