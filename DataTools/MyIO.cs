using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;

namespace ExcelReader
{
    public class MyIO
    {
        public static string[] BannedWords;
        public static SQLiteConnection sqlite_conn;

        private static readonly string DataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DataCompanion");

        private static readonly string ConfigDir = Path.Combine(DataDir, "Configurations\\configs.db");
        
        internal static string GetFileContents(string fileName)
        {
            return File.ReadAllText(Path.Combine(DataDir, fileName));
        }
        internal static async void WriteToCSV(string text)
        {
            await File.WriteAllTextAsync(Path.Combine(DataDir, "Output\\OutputFile.csv"), text);
        }
        internal static async void WriteToFile(string name, string text)
        {
            await File.WriteAllTextAsync(Path.Combine(DataDir, $"{name}.txt"), text);
        }

        internal static void PopulateBannedWords()
        {
            BannedWords = GetFileContents("BannedWords.txt").Split("\n");
        }
        internal static string[] PopulateAvailableFiles()
        {
            string[] files = Directory.GetFiles(Path.Combine(DataDir, "Input"));
            for (int i = 0; i < files.Length; i++)
            {
                var split = files[i].Split("\\");
                files[i] = split[split.Length - 1];
            }
            return files;
        }

        //////////////////////////////////////////       

        public static void CreateDBConnection()
        {
            sqlite_conn = new SQLiteConnection(@"URI=file:" + ConfigDir);
            sqlite_conn.Open();
        }

        private static bool CreateTable(string tableName)
        {
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();           
            sqlite_cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}';";
            if (sqlite_cmd.ExecuteScalar() == null)
            {
                sqlite_cmd.CommandText = $"CREATE TABLE {tableName} (" +
                    "Col1 INT, " + //Column Choice
                    "Col2 INT, " +  //Max Length
                    "Col3 INT, " +  //Min Length
                    "Col4 INT, " + // Min Length Fill Character index
                    "Col5 INT)";  // Banned Word Policy
                sqlite_cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        internal static void PopulateNewTable(string tableName)
        {
            if (CreateTable(tableName))
            {
                for (int column = 0; column < MyData.data.Count - 1; column++)
                {
                    InsertData(tableName, new string[] { column.ToString(), "0", "0", "0", "0" });
                }
            }
        }

        private static void InsertData(string tableName, string[] data)
        {
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO {tableName} (Col1, Col2, Col3, Col4, Col5) VALUES({data[0]}, {data[1]}, {data[2]}, '{data[3]}', {data[4]})";
            sqlite_cmd.ExecuteNonQuery();
        }

        ///////////////////////////////////////////////////

        internal static void UpdateBannedPolicy(string tableName, string policy, string column)
        {
            UpdateColumn(tableName, $"SET Col5 = {policy} WHERE Col1 = {column}");
        }

        internal static void UpdateMinLengthPolicy(string tableName, string length, string column)
        {
            UpdateColumn(tableName, $"SET Col3 = {length} WHERE Col1 = {column}");
        }
        internal static void UpdateMaxLengthPolicy(string tableName, string length, string column)
        {
            UpdateColumn(tableName, $"SET Col2 = {length} WHERE Col1 = {column}");
        }

        internal static void UpdateFillChar(string tableName, string fillChar, string column)
        {
            UpdateColumn(tableName, $"SET Col4 = {fillChar} WHERE Col1 = {column}");
        }

        ///////////////////////////////////////////////////

        internal static List<int> GetColumns(string tableName)
        {
            return GetColumn($"SELECT Col1 FROM {tableName}");
        }

        internal static List<int> GetBannedWordColumns(string tableName)
        {
            return GetColumn($"SELECT Col1 FROM {tableName} WHERE Col5 = 1");
        }

        internal static List<int> GetMinLengthColumns(string tableName)
        {
            return GetColumn($"SELECT Col1 FROM {tableName} WHERE Col3 > 0");
        }

        internal static int GetColumnsMinLength(string tableName, string column)
        {
            return GetColumn($"SELECT Col3 FROM {tableName} WHERE Col1 = {column}")[0];
        }

        internal static List<int> GetMaxLengthColumns(string tableName)
        {
            return GetColumn($"SELECT Col1 FROM {tableName} WHERE Col2 > 0");
        }

        internal static int GetColumnsMaxLength(string tableName, string column)
        {
            return GetColumn($"SELECT Col2 FROM {tableName} WHERE Col1 = {column}")[0];
        }

        internal static string GetColumnsFillChar(string tableName, string column)
        {
            return new string[] { "X", "$", "*", "#", "?", "_" }
                [GetColumn($"SELECT Col4 FROM {tableName} WHERE Col1 = {column}")[0]];
        }

        internal static List<int> GetColumnConfig(string tableName, string columnNumber)
        {
            if (columnNumber.Length == 0) { columnNumber = "0"; }
            return GetRow(
                $"SELECT Col2, Col3, Col4, Col5 FROM {tableName} WHERE Col1 = {columnNumber}",
                new int[] { 0, 1, 2, 3 });
        }

        internal static List<string> GetTableNames()
        {
            List<string> columns = new();
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                columns.Add(sqlite_datareader.GetString(0));
            }
            return columns;
        }

        internal static void DeleteTable(string tablename)
        {
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"DROP TABLE '{tablename}'";
            sqlite_cmd.ExecuteNonQuery();
        }

        ///////////////////////////////////////////////////

        private static List<int> GetColumn(string SQL)
        {
            List<int> columns = new();
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = SQL;
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                columns.Add(sqlite_datareader.GetInt32(0));
            }
            return columns;
        }

        private static List<int> GetRow(string SQL, int[] columns)
        {
            List<int> row = new();
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = SQL;
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                foreach (int i in columns)
                {
                    row.Add(sqlite_datareader.GetInt32(i));
                }
            }
            return row;
        }

        private static void UpdateColumn(string tableName, string SQL)
        {
            try
            {
                if (tableName.Length == 0) { return; }
                SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"UPDATE {tableName} {SQL}";
                sqlite_cmd.ExecuteNonQuery();
            }
            catch(Exception)
            {
                return;
            }
        }

        ///////////////////////////////////////////////////

        internal static void FirstTimeSetup()
        {
            if (!Directory.Exists(DataDir))
            {
                foreach (string dir in new List<string>
                { 
                    DataDir,
                    Path.Combine(DataDir, "Output"),
                    Path.Combine(DataDir, "Input"),
                    Path.Combine(DataDir, "Configurations")
                })
                {
                    Directory.CreateDirectory(dir);
                }
                WriteToFile("BannedWords", "poop");
            }
        }
    }
}
