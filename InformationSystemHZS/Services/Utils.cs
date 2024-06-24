using InformationSystemHZS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystemHZS.Services
{
    public static class Utils
    {
        public static readonly string IncidentStartTimeFormat = "dd.MM.yyyy HH:mm:ss";

        public static DateTime? FromIncidentStringStartTime(string stringStartTime)
        {
            DateTime startTime;
            if (!DateTime.TryParseExact(stringStartTime, IncidentStartTimeFormat, null, DateTimeStyles.None, out startTime))
            {
                return null;
            }

            return startTime;
        }

        public static int GetTypePaddingSize<T>() where T : Enum
        {
            int paddingSize = 0;
            foreach (string type in Enum.GetNames(typeof(T)))
            {
                if (type.Length > paddingSize)
                {
                    paddingSize = type.Length;
                }
            }

            return paddingSize;
        }

        public static string MinutesAndSeconds(int seconds)
        {
            int minutes = seconds / 60;
            seconds = seconds % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }

        public static string[] GetQuotesTokens(string text)
        {
            List<string> tokens = new List<string>();
            StringBuilder word = new StringBuilder();
            bool isQuotesWord = false;
            bool isWord = false;
            char? lastChar;
            foreach (char c in text)
            {
                lastChar = c;
                if (char.IsWhiteSpace(c))
                {
                    if (isQuotesWord)
                    {
                        word.Append(c);
                    }
                    else if (isWord)
                    {
                        tokens.Add(word.ToString());
                        isWord = false;
                        word.Clear();
                    }
                }
                else if (c == '"')
                {
                    if (isQuotesWord)
                    {
                        tokens.Add(word.ToString());
                        isQuotesWord = false;
                        word.Clear();
                    }
                    else if (isWord)
                    {
                        word.Append('"');
                    }
                    else
                    {
                        isQuotesWord = true;
                    }
                }
                else
                {
                    word.Append(c);
                    if (!isQuotesWord)
                    {
                        isWord = true;
                    }
                }
            }

            if (isWord || isQuotesWord)
            {
                tokens.Add(word.ToString());
            }

            return tokens.ToArray();
        }

        public static int? GetNumber(string text)
        {
            int num;
            if (!int.TryParse(text, out num))
            {
                return null;
            }

            return num;
        }
    }
}
