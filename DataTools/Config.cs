using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader.DataTools
{
    internal class Config
    {
        internal string name;
        internal List<int> columns;
        internal List<int> maxLengths;
        internal List<int> minLengths;
        internal List<int> fillChar;
        internal List<int> bannedWordPolicy;

        internal Config(string name)
        {
            this.name = name;
            this.columns = new List<int>();
            this.maxLengths = new List<int>();
            this.minLengths = new List<int>();
            this.fillChar = new List<int>();
            this.bannedWordPolicy = new List<int>();
        }

        
    }
}
