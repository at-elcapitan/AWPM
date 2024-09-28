using System;
using System.Collections.Generic;

namespace AWPM
{
    internal class ProgramFlags
    {
        private bool ask = false;
        private bool info = false;
        private bool update = false;
        private bool force = false;
        private bool upgrade = false;
        private bool install = false;
        private bool remove = false;
        private bool help = false;
        private bool version = false;
        private bool validated = false;
        private string target = "";
        private List<string> packages = new List<string>();

        public bool Ask
        {
            get => ask;
        }

        public bool Force
        {
            get => force;
        }

        public bool Help
        {
            get => help;
        }

        public bool Version
        {
            get => version;
        }

        public List<string> Packages
        {
            get => packages;
        }

#if DEBUG
        public override string ToString()
        {
            string ret = "--- DEBUG INFO ---\nOperations:\n";

            if (this.update && this.force)
            {
                ret += "\t" + Operations.FUPDATE.ToString() + "\n";
            }
            else if (this.update)
            {
                ret += "\t" + Operations.UPDATE.ToString() + "\n";
            }

            if (this.upgrade)
            {
                ret += "\t" + Operations.UPGRADE.ToString() + "\n";
            }

            if (this.install)
            {
                ret += "\t" + Operations.INSTALL.ToString() + "\n";
            }

            if (this.remove)
            {
                ret += "\t" + Operations.REMOVE.ToString() + "\n";
            }

            if (this.info)
            {
                ret += "\t" + Operations.INFO.ToString() + "\n";
            }

            ret += "Flags: ";
            if (this.ask)
            {
                ret += "ASK\n";
            }

            ret += "Packages:\n";

            foreach(string s in this.packages)
            {
                ret += "\t" + s + "\n";         
            }

            if (this.target != "")
            {
                ret += $"Upgrade target: {this.target}\n";
            }

            ret += "--- END ---\n\n";
            return ret;
        }
#endif

        public (bool, string) validateArguments()
        {
            if (!this.info && !this.ask && !this.update && !this.upgrade &&
                !this.install && !this.remove)
            {
                return (false, "No arguments provided.\nUse --help to display " +
                	"list of commands.");
            }

            if (this.info && (this.ask || this.upgrade ||
                              this.install || this.remove))
            {
                return (false, "Unable to execute command" +
                               " 'list' with other arguments");
            }

            if (this.install && this.remove)
            {
                return (false, "Unabl\t\te to remove packages" +
                               " due to installation command");
            }

            if (this.upgrade && this.remove)
            {
                return (false, "Unable to remove packages" +
                               " due to upgrade command");
            }

            if (this.upgrade && target == "")
            {
                return (false, "Found target argument," +
                    " with no upgrade operation provided");
            }

            this.validated = true;
            return (true, "");
        }

        public ProgramFlags(string[] args)
        {
            foreach (string el in args)
            {
                switch (el)
                {
                    case "--ask":
                        this.ask = true;
                        break;
                    case "--info":
                        this.info = true;
                        break;
                    case "--update":
                        this.update = true;
                        break;
                    case "--force":
                        this.force = true;
                        break;
                    case "--upgrade":
                        this.upgrade = true;
                        break;
                    case "--remove":
                        this.remove = true;
                        break;
                    case "--install":
                        this.install = true;
                        break;
                    case "--version":
                        this.version = true;
                        break;
                    case "--help":
                        this.help = true;
                        break;
                    default:
                        if (el.StartsWith("--", StringComparison.Ordinal))
                        {
                            throw new ArgumentException(
                                    $"Unknown argument '{el}'");
                        }

                        if (el.StartsWith("@", StringComparison.Ordinal))
                        {
                            if (this.target != "")
                            {
                                throw new ArgumentException(
                                    "Multiple target argument found");
                            }

                            this.target = el;
                            continue;
                        }

                        if (el.StartsWith("-", StringComparison.Ordinal))
                        {
                            foreach (char c in el.Substring(1))
                            {
                                switch (c)
                                {
                                    case 'a':
                                        this.ask = true;
                                        break;
                                    case 'I':
                                        this.info = true;
                                        break;
                                    case 'y':
                                        if (this.update)
                                        {
                                            this.force = true;
                                            break;
                                        }
                                        this.update = true;
                                        break;
                                    case 'u':
                                        this.upgrade = true;
                                        break;
                                    case 'i':
                                        this.install = true;
                                        break;
                                    case 'r':
                                        this.remove = true;
                                        break;
                                    default:
                                        throw new ArgumentException(
                                            "Unknown argument"
                                        );
                                }
                            }
                            break;
                        }

                        this.packages.Add(el);
                        break;
                }
            }
        }

        public List<Operations> generateTasklist()
        {
            if (!validated)
            {
                throw new InvalidOperationException("Internal exception, " +
                	"arguments not validated");
            }

            List<Operations> ret = new List<Operations>();

            if (this.update && this.force)
            {
                ret.Add(Operations.FUPDATE);
            } else if (this.update)
            {
                ret.Add(Operations.UPDATE);
            }

            if (this.upgrade)
            {
                ret.Add(Operations.UPGRADE);
            } 

            if (this.install)
            {
                ret.Add(Operations.INSTALL);
            }

            if (this.remove)
            {
                ret.Add(Operations.REMOVE);
            }

            if (this.info)
            {
                ret.Add(Operations.INFO);
            }

            return ret;
        }
    }
}
