using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace LogChecker
{
    public class IConfig : INotifyPropertyChanged
    {
        [JsonIgnore]
        private static string File { get; set; }

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private int _ThreadCount = 50;

        [JsonIgnore]
        public int ThreadCount
        {
            get => _ThreadCount;
            set
            {
                _ThreadCount = value;

                NotifyPropertyChanged(nameof(ThreadCount));
            }
        }

        private Dictionary<string, List<uint>> _MasterList = new Dictionary<string, List<uint>>();

        [JsonIgnore]
        public Dictionary<string, List<uint>> MasterList
        {
            get => _MasterList;
            set
            {
                _MasterList = value;

                NotifyPropertyChanged(nameof(MasterList));
            }
        }

        private string _Log;

        [JsonProperty]
        public string Log
        {
            get => _Log;
            set
            {
                _Log = value;

                NotifyPropertyChanged(nameof(Log));
            }
        }

        public bool ShouldSerializeLog() => !string.IsNullOrEmpty(Log);

        #region White List

        private List<uint> _WhiteList = new List<uint>();

        [JsonProperty("White List")]
        public List<uint> WhiteList
        {
            get => _WhiteList;
            set
            {
                _WhiteList = value;

                NotifyPropertyChanged(nameof(WhiteList));
                NotifyPropertyChanged(nameof(WhiteListPreview));
            }
        }

        public bool ShouldSerializeWhiteList() => WhiteList.Count > 0;

        [JsonIgnore]
        public string WhiteListPreview
        {
            get => string.Join(", ", WhiteList);
            set
            {

            }
        }

        #endregion

        #region Account List

        public class IASF
        {
            private string _IP;

            [JsonProperty]
            public string IP
            {
                get => _IP;
                set
                {
                    _IP = value;

                    NotifyPropertyChanged(nameof(IP));
                }
            }

            public bool ShouldSerializeIP() => !string.IsNullOrEmpty(IP);

            private string _Password;

            [JsonProperty]
            public string Password
            {
                get => _Password;
                set
                {
                    _Password = value;

                    NotifyPropertyChanged(nameof(IP));
                }
            }

            public bool ShouldSerializePassword() => !string.IsNullOrEmpty(Password);

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Dictionary<string, IASF> _AccountList = new Dictionary<string, IASF>();

        [JsonProperty("Account List")]
        public Dictionary<string, IASF> AccountList
        {
            get => _AccountList;
            set
            {
                _AccountList = value;

                NotifyPropertyChanged(nameof(AccountList));
                NotifyPropertyChanged(nameof(AccountListPreview));
            }
        }

        public bool ShouldSerializeAccountList() => AccountList.Count > 0;

        [JsonIgnore]
        public string AccountListPreview
        {
            get => string.Join(", ", AccountList.Select(x => x.Key));
            set
            {

            }
        }

        #endregion

        #region Checked

        public class IChecked
        {
            public IChecked(string EMail, DateTime Date, List<string> GameKeyList)
            {
                this.EMail = EMail;
                this.Date = Date;
                this.GameKeyList = GameKeyList ?? new List<string>();
            }

            [JsonProperty("E-Mail")]
            public string EMail { get; set; }

            public bool ShouldSerializeEMail() => !string.IsNullOrEmpty(EMail);

            [JsonProperty]
            public DateTime Date { get; set; } = DateTime.MinValue;

            public bool ShouldSerializeDate() => Date != DateTime.MinValue;

            [JsonProperty("Game Key List")]
            public List<string> GameKeyList { get; set; } = new List<string>();

            public bool ShouldSerializeGameKeyList() => GameKeyList.Count > 0;

        }

        private List<IChecked> _Checked = new List<IChecked>();

        [JsonProperty]
        public List<IChecked> Checked
        {
            get => _Checked;
            set
            {
                _Checked = value;

                NotifyPropertyChanged(nameof(Checked));
            }
        }

        public bool ShouldSerializeChecked() => Checked.Count > 0 && Checked.Any(x => x.ShouldSerializeEMail() || x.ShouldSerializeGameKeyList() || x.ShouldSerializeDate());

        #endregion

        public static (string ErrorMessage, IConfig Config) Load(string Directory, string _File)
        {
            if (!string.IsNullOrEmpty(Directory) && !System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            File = _File;

            if (!string.IsNullOrEmpty(File) && !System.IO.File.Exists(File))
            {
                System.IO.File.WriteAllText(File, JsonConvert.SerializeObject(new IConfig(), Formatting.Indented));
            }

            string Json;

            try
            {
                Json = System.IO.File.ReadAllText(File);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (string.IsNullOrEmpty(Json) || Json.Length == 0)
            {
                return ("Данные равны нулю!", null);
            }

            IConfig Config;

            try
            {
                Config = JsonConvert.DeserializeObject<IConfig>(Json);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (Config == null)
            {
                return ("Глобальный конфиг равен нулю!", null);
            }

            Config.Save();

            return (null, Config);
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(File) || (this == null)) return;

            string JSON = JsonConvert.SerializeObject(this, Formatting.Indented);
            string _ = File + ".new";

            Semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                System.IO.File.WriteAllText(_, JSON);

                if (System.IO.File.Exists(File))
                {
                    System.IO.File.Replace(_, File, null);
                }
                else
                {
                    System.IO.File.Move(_, File);
                }
            }
            catch
            {

            }
            finally
            {
                Semaphore.Release();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
