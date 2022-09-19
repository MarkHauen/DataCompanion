using System.Collections.Generic;

namespace ExcelReader
{
    internal static class MyData
    {
        internal static List<List<string>> data;

        internal static void BuildData(string inputData)
        {
            string[] rows = inputData.Trim().Split("\r\n");
            data = new List<List<string>> { };

            for (int row = 0; row <= (rows.Length - 1); row++)
            {
                data.Add(new List<string>(rows[row].Split(",")));
            }
        }

        internal static string OutputData()
        {
            string output = "";
            foreach (List<string> row in data)
            {
                output += string.Join(",", row) + "\r\n";
            }
            return output;
        }

        internal static void RunBannedWordRedact(int column)
        {
            for (int row = 0; row < data.Count; row++)
            {
                data[row][column] = Transformations.RedactBannedWord(data[row][column]);
            }
        }

        internal static void RunBannedWordRemove(int column)
        {
            for (int row = 0; row < data.Count; row++)
            {
                if (Transformations.ContainsBannedWord(data[row][column]))
                {
                    data.RemoveAt(row);
                    break;
                }
            }
        }

        internal static void RunEnsureMinLength(int column, int minLength, string fillChar)
        {
            for (int row = 0; row < data.Count; row++)
            {
                data[row][column] = Transformations.EnsureLengthWithFill(data[row][column], minLength, fillChar);
            }
        }

        internal static void RunEnsureMaxLength(int column, int maxLength)
        {
            for (int row = 0; row < data.Count; row++)
            {
                data[row][column] = Transformations.EnsureLengthNotExecded(data[row][column], maxLength);
            }
        }
    }
}
