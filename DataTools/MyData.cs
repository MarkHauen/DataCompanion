using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    public static class MyData
    {
        public static List<List<string>> data;

        public static void RunBannedWordRedact()
        {
            for(int row = 0; row < (data.Count - 1); row++)
            {
                for(int column = 0; column < (data.ElementAt(row).Count - 1); column++)
                {
                    data[row][column] = Transformations.RedactBannedWord(data[row][column]);
                }
            }
        }
        public static void RunBannedWordRemove()
        {
            for (int row = 0; row < (data.Count - 1); row++)
            {
                for (int column = 0; column < (data.ElementAt(row).Count - 1); column++)
                {
                    if (Transformations.ContainsBannedWord(data[row][column]))
                    {
                        data.RemoveAt(row);
                        break;
                    }
                }
            }
        }
    }
}
