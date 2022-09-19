﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    internal class DataFactory
    {
        internal static void BuildData(string inputData)
        {
            string[] rows = inputData.Trim().Split("\r\n");
            MyData.data = new List<List<string>> { };

            for (int row = 0; row <= (rows.Length - 1); row++)
            {
                MyData.data.Add(new List<string>(rows[row].Split(",")));
            }
        }

        internal static string OutputData()
        {
            string output = "";
            foreach (List<string> row in MyData.data)
            {
                output += string.Join(",", row) + "\r\n";
            }
            return output;
        }
    }
}
