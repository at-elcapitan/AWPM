using System;
using Microsoft.Data.Sqlite;

namespace AWPM
{
    internal class Database
    {
        private string connectionString;

        public Database(string connectionString)
        {
            this.connectionString = connectionString;

            initializeDatabase();
        }

        public T executeCommand<T>(string query, Func<SqliteCommand, T> commandFunc)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    return commandFunc(command);
                }
            }
        }

        public void executeQuery(string query, Action<SqliteDataReader> readAction)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        readAction(reader);
                    }
                }
            }
        }

        private void initializeDatabase()
        {
            string createPackagesTable = @"
            CREATE TABLE IF NOT EXISTS Packages (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                version_major INTEGER NOT NULL,
                version_minor INTEGER NOT NULL,
                version_patch INTEGER NOT NULL,
                version_build INTEGER NOT NULL,
            );";

            this.executeCommand(createPackagesTable, command => {
                command.ExecuteNonQuery();
                return 0;
            });

            string createIndexOnPackagesName = @"
            CREATE INDEX IF NOT EXISTS idx_Packages_Name ON Packages (Name);
            ";

            this.executeCommand(createIndexOnPackagesName, command => {
                command.ExecuteNonQuery();
                return 0;
            });

            string createDependenciesTable = @"
            CREATE TABLE IF NOT EXISTS Dependencies (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                dependentPackageId INTEGER NOT NULL,
                dependencyPackageId INTEGER NOT NULL,
                FOREIGN KEY (dependentPackageId) REFERENCES Packages(id),
                FOREIGN KEY (dependencyPackageId) REFERENCES Packages(id)
            );";

            this.executeCommand(createDependenciesTable, command => {
                command.ExecuteNonQuery();
                return 0;
            });
        }
    }
}
