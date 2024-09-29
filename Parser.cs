using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace AWPM
{
    public class SyntaxErrorException : Exception
    {
        public List<(int, string)> DataList { get; }

        public SyntaxErrorException(List<(int, string)> dataList)
            : base("An error occurred with the provided data.")
        {
            DataList = dataList;
        }
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

        private bool syntaticCheck(string[] text)
        {
            bool isValidated = true;

            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                //for (int j = i + 1; j < text.Length; j++)
                //{
                //    if (!line.TrimEnd().EndsWith("\\", StringComparison.Ordinal))
                //    {
                //        i = j;
                //        break;
                //    }

                //    line = line.Remove(line.Length - 1);
                //    line += text[j].TrimStart();
                //}


                if (line.Contains("[") && !line.Contains("]"))
                {
                    bool arrayValid = false;

                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (text[j].Contains("="))
                        {
                            break;
                        }

                        if (text[j].Contains("]"))
                        {
                            line += text[j].TrimStart();
                            i = j;
                            arrayValid = true;
                            break;
                        }

                        line += text[j].TrimStart();
                    }

                    if (!arrayValid)
                    {
                        notMatching.Add((i + 1, text[i].TrimStart() + 
                                        " --- [ was never closed"));
                        isValidated = false;
                        break;
                    }
                }

                if (!line.Contains("[") && line.Contains("]"))
                {
                    notMatching.Add((i + 1, text[i].TrimStart() + 
                                        " <- unexpected token"));
                    isValidated = false;
                    break;
                }

                bool match = false;

                if (this.variablePattern.IsMatch(line))
                {
                    match = true;
                    continue;
                }

                if (this.arrayPattern.IsMatch(line))
                {
                    match = true;
                    continue;
                }

                if (!match)
                {
                    notMatching.Add((i + 1, line.TrimStart() 
                        + $" <- unexpected token(s) starting {i + 1} line"));
                    isValidated = false;
                    break;
                }
            }

            return isValidated;
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
            if (!this.syntaticCheck(data.Split('\n')))
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
