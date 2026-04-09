using System;
using System.Data.SQLite;

namespace Group16_iPERMITAPP.Database
{
    public static class DbHelper
    {
        private static string connectionString = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "Group16_iPERMITDB.db;Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static void InitializeDatabase()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    // Enable foreign keys
                    cmd.CommandText = "PRAGMA foreign_keys = ON;";
                    cmd.ExecuteNonQuery();

                    // ENVIRONMENTALPERMIT table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS ENVIRONMENTALPERMIT (
                            permitID INTEGER PRIMARY KEY,
                            permitName TEXT NOT NULL,
                            permitFee REAL NOT NULL,
                            description TEXT
                        );";
                    cmd.ExecuteNonQuery();

                    // RE table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS RE (
                            REId TEXT PRIMARY KEY,
                            REPassword TEXT NOT NULL,
                            contactPersonName TEXT NOT NULL,
                            createdDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            REEmail TEXT NOT NULL,
                            organizationName TEXT NOT NULL,
                            organizationAddress TEXT NOT NULL
                        );";
                    cmd.ExecuteNonQuery();

                    // EO table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS EO (
                            EOId INTEGER PRIMARY KEY,
                            EOName TEXT NOT NULL,
                            EOPassword TEXT NOT NULL
                        );";
                    cmd.ExecuteNonQuery();

                    // Hardcoded EO record
                    cmd.CommandText = @"
                        INSERT OR IGNORE INTO EO (EOId, EOName, EOPassword)
                        VALUES (1, 'NOM NOM', 'PASSWORD');";
                    cmd.ExecuteNonQuery();

                    // RESITE table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS RESITE (
                            siteID TEXT PRIMARY KEY,
                            siteAddress TEXT NOT NULL,
                            siteContactPerson TEXT NOT NULL,
                            REId TEXT NOT NULL,
                            FOREIGN KEY (REId) REFERENCES RE(REId)
                        );";
                    cmd.ExecuteNonQuery();

                    // PermitRequest table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS PermitRequest (
                            requestNo TEXT PRIMARY KEY,
                            permitID INTEGER NOT NULL,
                            dateOfRequest TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            activityDescription TEXT NOT NULL,
                            activityStartDate TEXT NOT NULL,
                            activityDuration INTEGER NOT NULL,
                            permitFee REAL NOT NULL,
                            REId TEXT NOT NULL,
                            FOREIGN KEY (permitID) REFERENCES ENVIRONMENTALPERMIT(permitID),
                            FOREIGN KEY (REId) REFERENCES RE(REId)
                        );";
                    cmd.ExecuteNonQuery();

                    // Payment table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Payment (
                            paymentID TEXT PRIMARY KEY,
                            requestNo TEXT NOT NULL,
                            amountPaid REAL NOT NULL,
                            paymentDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            FOREIGN KEY (requestNo) REFERENCES PermitRequest(requestNo)
                        );";
                    cmd.ExecuteNonQuery();

                    // Decision table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Decision (
                            decisionID TEXT PRIMARY KEY,
                            requestNo TEXT NOT NULL,
                            decisionDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            decisionStatus TEXT NOT NULL,
                            decisionNotes TEXT,
                            FOREIGN KEY (requestNo) REFERENCES PermitRequest(requestNo)
                        );";
                    cmd.ExecuteNonQuery();

                    // Permit table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Permit (
                            permitNo TEXT PRIMARY KEY,
                            requestNo TEXT NOT NULL,
                            issueDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            expiryDate TEXT NOT NULL,
                            FOREIGN KEY (requestNo) REFERENCES PermitRequest(requestNo)
                        );";
                    cmd.ExecuteNonQuery();

                    // RequestStatus table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS RequestStatus (
                            statusID TEXT PRIMARY KEY,
                            requestNo TEXT NOT NULL,
                            statusDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            status TEXT NOT NULL,
                            FOREIGN KEY (requestNo) REFERENCES PermitRequest(requestNo)
                        );";
                    cmd.ExecuteNonQuery();

                    // EmailArchive table
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS EmailArchive (
                            emailID TEXT PRIMARY KEY,
                            recipientEmail TEXT NOT NULL,
                            emailDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            subject TEXT NOT NULL,
                            body TEXT NOT NULL
                        );";
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
