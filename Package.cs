using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AWPM
{
    internal enum ReturnStates
    {
        PREPEAR,
        INSTALL,
        POST_INSTALL,
        ERR_PREP,
        ERR_CHECKSUM,
        ERR_INSTALL_NO_DEST_DIR,
        ERR_POST_INSTALL,
        ERR_CLEAN,
        ERR_PACKAGE_ROOT_CONTATINS_FILES,
        ERR_PACKAGE_DIRS_VERIFICATION_FAILED,
        OK
    }

    internal struct Package
    {
        public string url;
        public string pacakgeName;
        public string packageVersion;
        public string tempWorkingDirectory;
        public string packageFileName;
        public ReturnStates state;
        public string errMessage;
        public byte[] checksum;
        bool optional;
        bool exists;
        // bool isSource;
    }

    internal class PackageInstaller
    {
        private List<Package> installPackages;
        private List<Package> dependencies;
        private string dataFolder;

        private void copyDirectory(string sourcedir, string destdir)
        {
            if (!Directory.Exists(destdir))
            {
                Directory.CreateDirectory(destdir);
            }

            foreach (var filePath in Directory.GetFiles(sourcedir))
            {
                File.Copy(filePath, Path.Combine(destdir,
                                    Path.GetFileName(filePath)));
            }

            foreach (var subDir in Directory.GetDirectories(sourcedir))
            {
                this.copyDirectory(subDir, Path.Combine(destdir, 
                                           Path.GetFileName(subDir)));
            }
        }

        private void postInstall(Package package)
        {

        }

        private ReturnStates check(Package package)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(package.tempWorkingDirectory 
                                                + package.packageFileName))
                {
                    byte[] hash = md5.ComputeHash(stream);

                    if (hash != package.checksum)
                    {
                        return ReturnStates.ERR_CHECKSUM;
                    }
                }
            }

            return ReturnStates.OK;
        }

        private ReturnStates installPackage (Package package)
        {
            string[] allowedDirs = { "bin", "lib", "opt" };
            // unpacking
            if (!Directory.Exists(this.dataFolder))
                return ReturnStates.ERR_INSTALL_NO_DEST_DIR;

            if (Directory.GetFiles(package.tempWorkingDirectory).Any())
            {
                return ReturnStates.ERR_PACKAGE_ROOT_CONTATINS_FILES;
            }

            var subdirs = Directory.GetDirectories(package.tempWorkingDirectory);

            if (!subdirs.All(dir => allowedDirs.Contains(dir)))
            {
                return ReturnStates.ERR_PACKAGE_DIRS_VERIFICATION_FAILED;
            }

            this.copyDirectory(package.tempWorkingDirectory, this.dataFolder);

            return ReturnStates.OK;
        }

        private void build(Package package)
        {

        }

        private void prepear(Package package)
        {

        }

        private void get(Package package)
        {

        }

        private void clean()
        {
            foreach (var package in this.installPackages)
            {
                Directory.Delete(package.tempWorkingDirectory, true);
            }
        }

        public (string, ReturnStates) install()
        {
            foreach (Package package in dependencies)
            {
                this.prepear(package);

                if (package.state != ReturnStates.PREPEAR)
                {
                    return (package.errMessage, package.state);
                }

                this.installPackage(package);

                if (package.state != ReturnStates.INSTALL)
                {
                    return (package.errMessage, package.state);
                }

                this.postInstall(package);

                if (package.state != ReturnStates.POST_INSTALL)
                {
                    return (package.errMessage, package.state);
                }
            }

            string s = this.clean();

            if (s != "")
            {
                return (s, ReturnStates.ERR_CLEAN);
            }
            return ("", ReturnStates.OK);
        }

        public PackageInstaller(List<Package> packages, List<Package> deps)
        {
            this.installPackages = packages;
            this.dependencies = deps;
        }
    }
}
