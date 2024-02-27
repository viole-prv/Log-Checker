using System.Collections.Generic;
using System.Globalization;

namespace LogChecker
{
    public class ILog
    {
        public string DirectoryValue { get; set; }

        #region Cookie List

        public class ICookieList
        {
            public ICookieList(string FileValue, List<ICookie> List)
            {
                this.FileValue = FileValue;
                this.List = List;
            }

            public string FileValue { get; set; }
            public List<ICookie> List { get; set; }
        }

        public class ICookie
        {
            public string Domain { get; set; }
            public double? ExpirationDate { get; set; }
            public bool? HttpOnly { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public bool? Secure { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Domain
                    + "\t"
                    + (HttpOnly?.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE")
                    + "\t"
                    + Path
                    + "\t"
                    + (Secure?.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE")
                    + "\t"
                    + ExpirationDate
                    + "\t"
                    + Name
                    + "\t"
                    + Value;
            }
        }

        #endregion

        public List<ICookieList> Cookie { get; set; } = new List<ICookieList>();

        #region Password List

        public class IPasswordList
        {
            public string URL { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        #endregion

        public List<IPasswordList> Password { get; set; } = new List<IPasswordList>();
    }
}
