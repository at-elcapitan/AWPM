using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace AWPM
{
    internal class SyntaxErrorException : Exception
    {
        public List<(int, string)> DataList { get; }

        public SyntaxErrorException(List<(int, string)> dataList)
            : base("An error occurred with the provided data.")
        {
            DataList = dataList;
        }
    }

    internal enum EListValidator
    {
        LIST_NOT_CLOSED,
        LIST_CHILD_SYNTAX,
        UNKNOWN,
        OK
    }

    internal class Parser
    {
        private Regex variablePattern = new Regex(@"(\w+)\s*=\s*""([^""]*)""");
        private Regex arrayPattern = new Regex(@"(\w+)\s*=\s*\[\s*((?:\s*""[^""]*""(?:,\s*)?)+)\s*\]");

        private Dictionary<string, string> variables =       
                            new Dictionary<string, string>();
        private Dictionary<string, List<string>> lists =
                            new Dictionary<string, List<string>>();
        private List<(int line, string text)> notMatching =
                            new List<(int line, string text)>();

        public Dictionary<string, string> Variables
        {
            get => variables;
        }

        public Dictionary<string, List<string>> Lists
        {
            get => lists;
        }

        private bool syntacticCheck(string[] text)
        {
            var errbuf = new List<(int, string)>();

            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].TrimStart();

                if (string.IsNullOrWhiteSpace(line)) continue;

                switch (validateList(text, ref i, line, errbuf))
                {
                    case EListValidator.LIST_NOT_CLOSED:
                    case EListValidator.LIST_CHILD_SYNTAX:
                        notMatching.AddRange(errbuf);
                        return false;
                    case EListValidator.OK:
                        continue;
                    case EListValidator.UNKNOWN:
                        break;
                }

                if (!this.variablePattern.IsMatch(line))
                {
                    notMatching.Add((i + 1, $"{line} <-- unexpected token"));
                    return false;
                }
            }

            return true;
        }

        private EListValidator validateList(string[] text, ref int i, 
                                      string line, List<(int, string)> errbuf)
        {
            if (line.Contains("[") && line.Contains("]") 
                && !this.arrayPattern.IsMatch(line))
            {
                if (line.Count(c => c == '"') % 2 != 0)
                {
                    errbuf.Add((i + 1, $"\t{line}" +
                                        " <-- quote must be closed"));
                    return EListValidator.LIST_CHILD_SYNTAX;
                }

                return EListValidator.UNKNOWN;
            }

            if (line.Contains("[") && !line.Contains("]"))
            {
                errbuf.Add((i + 1, line));

                for (int j = i + 1; j < text.Length; j++)
                {
                    string processLineBuf = text[j].TrimStart();

                    if (processLineBuf.Contains("=")) break;

                    if (processLineBuf.Count(c => c == '"') % 2 != 0)
                    {
                        errbuf.Add((j + 1, $"\t{processLineBuf}" +
                                            " <-- quote must be closed"));
                        return EListValidator.LIST_CHILD_SYNTAX;
                    }

                    if (processLineBuf.Contains("]"))
                    {
                        line += processLineBuf;
                        errbuf.Add((j + 1, "\t" + processLineBuf));
                        i = j;
                        return EListValidator.OK;
                    }

                    errbuf.Add((j + 1, "\t" + processLineBuf));
                    line += processLineBuf;
                }

                var firstEntry = errbuf[0];
                errbuf[0] = (firstEntry.Item1, firstEntry.Item2 + 
                                "<-- bracket was never closed");
                return EListValidator.LIST_NOT_CLOSED;
            }

            if (!line.Contains("[") && line.Contains("]"))
            {
                return EListValidator.UNKNOWN;
            }

            return EListValidator.OK;
        }

        private void parseData(string text)
        {
            foreach (Match match in this.variablePattern.Matches(text))
            {
                this.variables[match.Groups[1].Value] = match.Groups[2].Value
                                                                  .Trim('"');
            }

            foreach (Match match in arrayPattern.Matches(text))
            {
                List<string> items = new List<string>();

                var itemMatches = Regex.Matches(match.Groups[0].Value, "\"([^\"]*)\"");

                foreach (Match itemMatch in itemMatches)
                {
                    items.Add(itemMatch.Groups[1].Value);
                }

                lists[match.Groups[1].Value] = items;
            }
        }

#if DEBUG
        public Parser(string data)
        {
            if (!this.syntacticCheck(data.Split('\n')))
            {
                throw new SyntaxErrorException(this.notMatching);
            }
            this.parseData(data);
        }
#else
        //public Parser(string relativePath)
        //{
        //    if(!this.validateData(File.ReadAllLines(relativePath)))
        //    {
        //        throw new FormatException("Found some unvalid strings");
        //    }

        //    this.parseData(File.ReadAllText(relativePath));
        //}
#endif
    }
}
