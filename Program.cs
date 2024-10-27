using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace AWPM
{
    class MainClass
    {
#if DEBUG
        const string tempDir = "temp";
        const string binDir = "bin";
#endif

        private static (bool OK, string err) checkFilesystem() {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(exePath + tempDir))
            {
                return (false, "Unable to find temp dir");
            }

            if(!Directory.Exists(exePath + binDir))
            {
                return (false, "Unable to find bin dir");
            }

            return (true, "");
        }

        public static void Main(string[] args)
        {
            //ProgramFlags flags;

            //try
            //{
            //    flags = new ProgramFlags(args);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    return;
            //}

            (bool, string) filesystem = checkFilesystem();

            if (!filesystem.Item1)
            {
                Console.WriteLine("Fatal error {0}", filesystem.Item2);
                return;
            }
#if DEBUG
            string[] debugArgs = { "-i", "package1" };
            ProgramFlags flags = new ProgramFlags(debugArgs);
            Console.Write(flags);
#endif

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



            Package package = new Package();
            package.pacakgeName;
        }
    }
}
