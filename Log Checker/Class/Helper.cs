using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LogChecker
{
    public partial class Helper
    {
        public static double GetDoubleValue(string Value)
        {
            double NewValue = 0.0;

            if (!string.IsNullOrEmpty(Value))
            {
                try
                {
                    Value = Value.Trim();

                    var Double = Regex.Match(Value, @"([0-9]+\W+[0-9]+)");

                    if (Double.Success &&
                        Double.Groups[1].Success)
                    {
                        NewValue = double.Parse(Double.Groups[1].Value.Replace(",", "."));
                    }
                    else
                    {
                        var Integer = Regex.Match(Value, @"([0-9]+)");

                        if (Integer.Success &&
                            Integer.Groups[1].Success)
                        {
                            NewValue = double.Parse(Integer.Groups[1].Value);
                        }
                    }
                }
                catch { }
            }

            return NewValue;
        }

        #region Random

        private static readonly Random Random = new Random();

        public static int Next(int Min, int Max) => Random.Next(Min, Max);

        #endregion

        public static void Shredder(DirectoryInfo X)
        {
            if (X.Exists)
            {
                foreach (DirectoryInfo _ in X.EnumerateDirectories())
                {
                    Shredder(_);
                }

                foreach (FileInfo _ in X.EnumerateFiles())
                {
                    _.Attributes = FileAttributes.Normal;
                    _.IsReadOnly = false;

                    _.Delete();
                }

                X.Delete();
            }
        }
    }
}
