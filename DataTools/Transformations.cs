using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ExcelReader
{
    class Transformations
    {
        public static string EnsureLengthNotExecded(string data, int maxLength)
        {
            if (data.Length > maxLength)
            {
                return data.Substring(0, maxLength);
            }
            return data;
        }
        public static string EnsureLengthWithFill(string data, int maxLength)
        {
            if (data.Length > maxLength)
            {
                return data.Substring(0, maxLength);
            }
            while (data.Length != maxLength)
            {
                data += "0";
            }
            return data;
        }
        public static string RedactBannedWord(string data)
        {
            foreach (string BadWord in MyIO.BannedWords)
            {
                if (data == BadWord)
                {
                    return "$$BANNEDWORD$$";
                }
            }
            return data;
        }
        public static bool ContainsBannedWord(string data)
        {
            foreach (string BadWord in MyIO.BannedWords)
            {
                if (data == BadWord)
                {
                    return true;
                }
            }
            return false;
        }

        internal static void BannedWordCheck(CheckBox bannedWordCheck, ComboBox bannedWordPolicyChoice)
        {
            if (bannedWordCheck.IsChecked.Value)
            {
                if (bannedWordPolicyChoice.Text == "Replace")
                {
                    MyData.RunBannedWordRedact();
                }
                else if (bannedWordPolicyChoice.Text == "Delete Row")
                {
                    MyData.RunBannedWordRemove();
                }
            } 
            
        }
    }
}
