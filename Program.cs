using System;
using System.Reflection;
using System.Collections.Generic;

namespace AWPM
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ProgramFlags flags;

            try
            {
                flags = new ProgramFlags(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (flags.Version)
            {
                Version version = Assembly.GetExecutingAssembly()
                                         .GetName().Version;
                Console.WriteLine($"Ado Windows Package Manager v" +
                    version + "\n\nThis is free software; see the source for " +
                    	"copying conditions. There is NO\nwarranty; not even " +
                    	"for MERCHANTABILITY or FITNESS FOR A PARTICULAR " +
                    	"PURPOSE.\n\nWritten by Vladislav 'ElCapitan' Nazarov");
                return;
            }

            (bool, string) valid = flags.validateArguments();

            if (!valid.Item1)
            {
                Console.WriteLine($"Error: {valid.Item2}\n");
                Console.WriteLine("Command failed, exiting...");
                return;
            }

            List<Operations> ops = flags.generateTasklist();

            foreach (Operations operation in ops)
            {
                Console.WriteLine(operation);   
            }

            foreach (string s in flags.Packages)
            {
                Console.WriteLine(s);
            }

        }
    }
}
