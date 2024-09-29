using System;
using System.Reflection;
using System.Collections.Generic;

namespace AWPM
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //            ProgramFlags flags;

            //            try
            //            {
            //                flags = new ProgramFlags(args);
            //            }
            //            catch (Exception e)
            //            {
            //                Console.WriteLine(e.Message);
            //                return;
            //            }

            //#if DEBUG
            //            Console.Write(flags);
            //#endif

            //    if (flags.Version)
            //    {
            //        Version version = Assembly.GetExecutingAssembly()
            //                                 .GetName().Version;
            //        Console.WriteLine($"Ado Windows Package Manager v" +
            //            version + "\n\nThis is free software; see the source for " +
            //            	"copying conditions. There is NO\nwarranty; not even " +
            //            	"for MERCHANTABILITY or FITNESS FOR A PARTICULAR " +
            //            	"PURPOSE.\n\nWritten by Vladislav 'ElCapitan' Nazarov");
            //        return;
            //    }

            //    (bool, string) valid = flags.validateArguments;

            //    if (!valid.Item1)
            //    {
            //        Console.WriteLine($"Error: {valid.Item2}\n");
            //        Console.WriteLine("Command failed, exiting...");
            //        return;
            //    }

            //    List<Operations> ops = flags.generateTasklist();

            //    foreach (Operations operation in ops)
            //    {
            //        Console.WriteLine(operation);   
            //    }

            //    foreach (string s in flags.Packages)
            //    {
            //        Console.WriteLine(s);
            //    }
#if DEBUG
            string input = @"
                    test = ""45""
                    type            = ""bin""
                    program_name    = ""pr ew fer erg rogram""
                    program_author  = ""elcapitan""
                    program_site    = ""https://github.com/at-elcapitan/AWPM""
                    program_version = ""1.1.1""
                    optional_deps   = [
                            ""progr:K|am3 : something"", 
                            ""1231,2""
                            ]
                  ";    
            Parser parser;

            try
            {
                parser = new Parser(input);

                foreach (var kvp in parser.Variables)
                {
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
                }

                foreach(var kvp in parser.Lists)
                {
                    Console.Write($"{kvp.Key} = [ ");

                    foreach (string s in kvp.Value)
                    {
                        Console.Write($"\"{s}\" ");
                    }

                    Console.Write("]\n");
                }
            } catch (SyntaxErrorException e)
            {
                Console.WriteLine("Syntax error");

                foreach((int, string) errline in e.DataList)
                {
                    Console.Write($"- line {errline.Item1}: {errline.Item2}");
                }
            }
#endif
        }
    }
}
