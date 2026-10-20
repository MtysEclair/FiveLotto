
using Microsoft.Data.Sqlite;
using System.Text;

namespace FiveLotto
{
    public static class CsvImporter
    {
        public static int ImportGamers(
            string filePath,
            SqliteConnection connection,
            SqliteTransaction transaction)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            int imported = 0;

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                "INSERT INTO Gamers (id, name) VALUES (@id, @name);";

            var idParameter = command.Parameters.Add("@id", SqliteType.Integer);
            var nameParameter = command.Parameters.Add("@name", SqliteType.Text);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] fields = lines[i].Split(';');

                if (fields.Length != 1)
                    throw new FormatException(
                        $"Hibás adatsor a gamers.csv fájlban: {i + 1}. sor.");

                string name = fields[0].Trim();

                if (name.Length == 0 || name.Length > 60)
                    throw new FormatException(
                        $"Érvénytelen név a gamers.csv fájl {i + 1}. sorában.");

                idParameter.Value = imported + 1;
                nameParameter.Value = name;

                command.ExecuteNonQuery();
                imported++;
            }

            return imported;
        }

        public static int ImportData(
            string filePath,
            SqliteConnection connection,
            SqliteTransaction transaction)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            int imported = 0;

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO Data
                    (gamersId, sz1, sz2, sz3, sz4, sz5)
                VALUES
                    (@gamersId, @sz1, @sz2, @sz3, @sz4, @sz5);";

            var gamersId = command.Parameters.Add("@gamersId", SqliteType.Integer);
            var sz1 = command.Parameters.Add("@sz1", SqliteType.Integer);
            var sz2 = command.Parameters.Add("@sz2", SqliteType.Integer);
            var sz3 = command.Parameters.Add("@sz3", SqliteType.Integer);
            var sz4 = command.Parameters.Add("@sz4", SqliteType.Integer);
            var sz5 = command.Parameters.Add("@sz5", SqliteType.Integer);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] fields = lines[i].Split(';');

                if (fields.Length != 6)
                    throw new FormatException(
                        $"Hibás adatsor a data5.csv fájlban: {i + 1}. sor.");

                int[] values = new int[6];

                for (int j = 0; j < fields.Length; j++)
                {
                    if (!int.TryParse(fields[j].Trim(), out values[j]))
                        throw new FormatException(
                            $"Hibás szám a data5.csv fájl {i + 1}. sorában.");
                }

                if (values[0] < 1 || values[0] > 6)
                    throw new FormatException(
                        $"Érvénytelen játékosazonosító a(z) {i + 1}. sorban.");

                for (int j = 1; j < values.Length; j++)
                {
                    if (values[j] < 1 || values[j] > 90)
                        throw new FormatException(
                            $"A lottószámoknak 1 és 90 közé kell esniük. Sor: {i + 1}.");
                }

                gamersId.Value = values[0];
                sz1.Value = values[1];
                sz2.Value = values[2];
                sz3.Value = values[3];
                sz4.Value = values[4];
                sz5.Value = values[5];

                command.ExecuteNonQuery();
                imported++;
            }

            return imported;
        }
    }
}
