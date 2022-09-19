using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ExcelReader
{
    internal class Transformations
    {
        internal static string EnsureLengthNotExecded(string data, int maxLength)
        {
            return data.Length > maxLength ? data.Substring(0, maxLength) : data;
        }

        internal static string EnsureLengthWithFill(string data, int maxLength, string fillChar)
        {
            if (data.Length > maxLength)
            {
                return data.Substring(0, maxLength);
            }
            while (data.Length != maxLength)
            {
                data += fillChar;
            }
            return data;
        }

        internal static string RedactBannedWord(string data)
        {
            foreach (string BadWord in MyIO.BannedWords)
            {
                if (data == BadWord.Trim())
                {
                    return "$$BANNEDWORD$$";
                }
            }
            return data;
        }

        internal static bool ContainsBannedWord(string data)
        {
            foreach (string BadWord in MyIO.BannedWords)
            {
                if (data == BadWord.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        internal static void BannedWordCheck(ComboBox bannedWordPolicyChoice, int column)
        {
            if (bannedWordPolicyChoice.Text == "Redact")
            {
                MyData.RunBannedWordRedact(column);
            }
            else
            {
                MyData.RunBannedWordRemove(column);
            }
        }
    }
}
