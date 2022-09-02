using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using ExcelReader.DataTools;

namespace ExcelReader
{
    class MyIO
    {
        public static string[] BannedWords;

        private static string DataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DataCompanion");

        static string ConfigDir = Path.Combine(DataDir, "Configurations\\configs.db");
        
        public static string GetFileContents(string fileName)
        {
            return File.ReadAllText(Path.Combine(DataDir, fileName));
        }
        public static async Task WriteToCSV(string text)
        {
            await File.WriteAllTextAsync(Path.Combine(DataDir, "Output\\OutputFile.csv"), text);
        }
        public static async Task WriteToFile(string text)
        {
            await File.WriteAllTextAsync(Path.Combine(DataDir, "Output\\OutputFile.txt"), text);
        }

        public static void PopulateBannedWords()
        {
            BannedWords = GetFileContents("BannedWords.txt").Split("\n");
        }

        //////////////////////////////////////////
        public static Config InitializeData()
        {
            SQLiteConnection sqlite_conn = CreateConnection();
            BuildDefaultTable(sqlite_conn);
            //CreateTable(sqlite_conn);
            //InsertData(sqlite_conn);
            return GetConfig(sqlite_conn, "DefaultTable");
        }

        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn = new SQLiteConnection(@"URI=file:" + ConfigDir);
            sqlite_conn.Open();
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn, string TableName)
        {
            string Createsql = "CREATE TABLE " + TableName + " (" +
                "Col1 INT, " + //Column Choice
                "Col2 INT, " +  //Max Length
                "Col3 INT, " +  //Min Length
                "Col4 INT, " + // Min Length Fill Character
                "Col5 INT)";  // Banned Word Policy
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        static void BuildDefaultTable(SQLiteConnection conn)
        {
            CreateTable(conn, "DefaultTable");
            InsertData(conn, "DefaultTable", new string[] { "0", "0", "0", "0", "1" });
            InsertData(conn, "DefaultTable", new string[] { "1", "0", "0", "0", "1" });
            InsertData(conn, "DefaultTable", new string[] { "2", "0", "0", "0", "1" });
        }

        static void InsertData(SQLiteConnection conn, string tableName, string[] data)
        {
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO " + tableName + "(Col1, Col2, Col3, Col4, Col5) VALUES(" +
                data[0] + ", " +
                data[1] + ", " +
                data[2] + ", " +
                data[3] + ", " +
                data[4] + ")";
            sqlite_cmd.ExecuteNonQuery();
        }

        public static Config GetConfig(SQLiteConnection conn, string tableName)
        {
            Config config = new Config(tableName);
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM " + tableName;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                config.columns.Add(sqlite_datareader.GetInt32(0));
                config.maxLengths.Add(sqlite_datareader.GetInt32(1));
                config.minLengths.Add(sqlite_datareader.GetInt32(2));
                config.fillChar.Add(sqlite_datareader.GetInt32(3));
                config.bannedWordPolicy.Add(sqlite_datareader.GetInt32(4));
            }
            conn.Close();
            return config;
        }
    }
}
