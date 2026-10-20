
using Microsoft.Data.Sqlite;

namespace FiveLotto
{
    public static class Database
    {
        public static string DbPath =>
            Path.Combine(AppContext.BaseDirectory, "otoslotto.db");

        public static SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(
                $"Data Source={DbPath}");

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();

            return connection;
        }

        public static void CreateTables()
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Gamers (
                    id INTEGER PRIMARY KEY,
                    name TEXT NOT NULL CHECK(length(name) <= 60)
                );

                CREATE TABLE IF NOT EXISTS Data (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    gamersId INTEGER NOT NULL,
                    sz1 INTEGER NOT NULL,
                    sz2 INTEGER NOT NULL,
                    sz3 INTEGER NOT NULL,
                    sz4 INTEGER NOT NULL,
                    sz5 INTEGER NOT NULL,
                    FOREIGN KEY (gamersId)
                        REFERENCES Gamers(id)
                );
            ";

            command.ExecuteNonQuery();
        }
    }
}
