
using Microsoft.Data.Sqlite;

namespace FiveLotto
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2 ||
                !File.Exists(args[0]) ||
                !File.Exists(args[1]))
            {
                Console.WriteLine("Az alkalmazás helyes használata:");
                Console.WriteLine(
                    "FiveLotto.exe <gamers.csv elérési útja> <data5.csv elérési útja>");
                return;
            }

            try
            {
                Database.CreateTables();

                using var connection = Database.GetConnection();
                using var transaction = connection.BeginTransaction();

                try
                {
                    using (var clear = connection.CreateCommand())
                    {
                        clear.Transaction = transaction;
                        clear.CommandText = @"
                            DELETE FROM Data;
                            DELETE FROM Gamers;
                        ";
                        clear.ExecuteNonQuery();
                    }

                    int gamersCount = CsvImporter.ImportGamers(
                        args[0], connection, transaction);

                    int dataCount = CsvImporter.ImportData(
                        args[1], connection, transaction);

                    transaction.Commit();

                    using (var check = connection.CreateCommand())
                    {
                        check.CommandText = @"
        SELECT
            (SELECT COUNT(*) FROM Gamers),
            (SELECT COUNT(*) FROM Data);";

                        using var reader = check.ExecuteReader();

                        if (reader.Read())
                        {
                            Console.WriteLine(
                                $"Adatbázis ellenőrzése: Gamers = {reader.GetInt32(0)}, " +
                                $"Data = {reader.GetInt32(1)}");
                        }
                    }

                    using (var check = connection.CreateCommand())
                    {
                        check.CommandText = "PRAGMA foreign_key_check;";

                        using var reader = check.ExecuteReader();

                        if (reader.Read())
                        {
                            Console.WriteLine(
                                "Figyelem: hibás idegenkulcs-kapcsolat található!");
                        }
                        else
                        {
                            Console.WriteLine(
                                "Idegenkulcs-kapcsolatok ellenőrzése: rendben.");
                        }
                    }

                    Console.WriteLine(
                        $"Sikeresen importált játékosok: {gamersCount} sor.");

                    Console.WriteLine(
                        $"Sikeresen importált tippek: {dataCount} sor.");

                    Console.WriteLine(
                        $"Adatbázis helye: {Database.DbPath}");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt az importálás során:");
                Console.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
        }
    }
}
