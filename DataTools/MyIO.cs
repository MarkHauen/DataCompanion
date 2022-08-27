using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

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
        public static async Task WriteToFile(string text)
        {
            await File.WriteAllTextAsync(Path.Combine(DataDir, "Output\\OutputFile.csv"), text);
        }

        public static void PopulateBannedWords()
        {
            BannedWords = GetFileContents("BannedWords.txt").Split("\n");
        }

        //////////////////////////////////////////
        static void initializeData(string[] args)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            //CreateTable(sqlite_conn);
            InsertData(sqlite_conn);
            ReadData(sqlite_conn);
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection(@"URI=file:" + ConfigDir);
               //"Data Source= database.db; Version = 3; New = True; Compress = True;");
           // Open the connection:
         try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn, string TableName)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE " + TableName + "(" +
                "Col1 INT, " + //Column Choice
                "Col2 INT, " +  //Max Length
                "Col3 INT, " +  //Min Length
                "Col4 VARCHAR(1), " + // Min Length Fill Character
                ", " + //
                ", " +
                ", " +
                ", " +
                ", " +
                ")";

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        static void InsertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test Text ', 1); ";
            sqlite_cmd.ExecuteNonQuery();
        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }


    }
}
