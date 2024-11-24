CREATE TABLE IF NOT EXISTS Packages (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    version_major INTEGER NOT NULL,
    version_minor INTEGER NOT NULL,
    version_patch INTEGER NOT NULL,
    version_build INTEGER NOT NULL,
);

CREATE INDEX IF NOT EXISTS idx_Packages_Name ON Packages (Name);

CREATE TABLE IF NOT EXISTS Dependencies (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    dependentPackageId INTEGER NOT NULL,
    dependencyPackageId INTEGER NOT NULL,
    FOREIGN KEY (dependentPackageId) REFERENCES Packages(id),
    FOREIGN KEY (dependencyPackageId) REFERENCES Packages(id)
);