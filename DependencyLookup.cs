using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace AWPM
{
    internal class DependencyLookup
    {
        private Database database;

        public DependencyLookup(Database database)
        {
            this.database = database;
        }

        public (string, List<Package>) calculateDependencies(List<string> packageNames)
        {
            List<Package> packages = new List<Package>();
            List<string> missingPackages = new List<string>();

            foreach (var packageName in packageNames)
            {
                var package = this.getPackageFromDatabase(packageName);

                if (package != null)
                {
                    packages.Add(package.Value);
                    var dependencies = this.getDependencies(package.Value.packageName);
                }
                else
                {
                    missingPackages.Add(packageName);
                }
            }

            if (missingPackages.Count > 0)
            {
                return (string.Join(", ", missingPackages), null);
            }

            return ("", packages);
        }

        private Package? getPackageFromDatabase(string packageName)
        {
            string query = "SELECT id, name, version_major, version_minor, " +
                          $"version_patch, version_build FROM Packages WHERE name = {packageName}";
            Package? package = null;

            database.executeQuery(query, reader =>
            {
                if (reader.Read())
                {
                    package = new Package
                    {
                        packageName = reader.GetString(1),
                        versionMajor = reader.GetInt32(2),
                        versionMinor = reader.GetInt32(3),
                        versionPatch = reader.GetInt32(4),
                        versionBuild =  reader.GetInt32(5),
                        url = reader.GetString(6),
                        checksum = reader["checksum"] as byte[],
                        state = ReturnStates.PACKAGE_SETUP
                    };
                }
            });

            return package;
        }

        private List<Package> getDependencies(string packageName)
        {
            List<Package> dependencies = new List<Package>();

            string query =
                "SELECT p.name, p.version_major, p.version_minor, p.version_patch, p.version_build, " +
                "p.url, p.checksum " +
                "FROM Dependencies d " +
                "JOIN Packages p ON d.dependencyPackageId = p.id " +
                "JOIN Packages pkg ON d.dependentPackageId = pkg.id " +
                $"WHERE pkg.name = {packageName}; ";

            database.executeQuery(query, reader =>
            {
                dependencies.Add(new Package
                {
                    packageName = reader.GetString(1),
                    versionMajor = reader.GetInt32(2),
                    versionMinor = reader.GetInt32(3),
                    versionPatch = reader.GetInt32(4),
                    versionBuild = reader.GetInt32(5),
                    url = reader.GetString(6),
                    checksum = reader["checksum"] as byte[],
                    state = ReturnStates.PACKAGE_SETUP
                });
            });

            return dependencies;
        }
    }
}
