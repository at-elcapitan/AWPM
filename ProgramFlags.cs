using System;
using System.Collections.Generic;

namespace AWPM
{
    internal class ProgramFlags
    {
        private bool ask = false;
        private bool list = false;
        private bool update = false;
        private bool force = false;
        private bool upgrade = false;
        private bool install = false;
        private bool remove = false;
        private bool help = false;
        private bool version = false;
        private bool validated = false;
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

        public (bool, string) validateArguments()
        {
            if (!this.list && !this.ask && !this.update && !this.upgrade &&
                !this.install && !this.remove)
            {
                return (false, "No arguments provided.\nUse --help to display " +
                	"list of commands.");
            }

            if (this.list && (this.ask || this.update || this.upgrade ||
                              this.install || this.remove))
            {
                return (false, "Unable to execute command" +
                               " 'list' with other arguments");
            }

            if (this.install && this.remove)
            {
                return (false, "Unable to remove packages" +
                               " due to installation command");
            }

            if (this.upgrade && this.remove)
            {
                return (false, "Unable to remove packages " +
                               " due to upgrade command");
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
                    case "--list":
                        this.list = true;
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
                            throw new ArgumentException("Unknown argument");
                        }

                        if (el.StartsWith("-", StringComparison.Ordinal))
                        {
                            foreach (char c in el)
                            {
                                switch (c)
                                {
                                    case 'a':
                                        this.ask = true;
                                        break;
                                    case 'l':
                                        this.list = true;
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

                            this.packages.Add(el);
                        }
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

            if (this.upgrade && this.install || this.upgrade && !this.install)
            {
                ret.Add(Operations.UPGRADE);
            } else if (this.install)
            {
                ret.Add(Operations.INSTALL);
            }

            if (this.remove)
            {
                ret.Add(Operations.REMOVE);
            }

            if (this.list)
            {
                ret.Add(Operations.LIST);
            }

            return ret;
        }
    }
}
