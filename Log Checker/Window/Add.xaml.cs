using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace LogChecker
{
    public partial class Add
    {
        #region Auto

        public class IAuto : INotifyPropertyChanged
        {
            private string _Key;

            public string Key
            {
                get => _Key;
                set
                {
                    _Key = value;

                    NotifyPropertyChanged(nameof(Key));
                }
            }

            private string _IP;

            public string IP
            {
                get => _IP;
                set
                {
                    if (!string.IsNullOrEmpty(value) &&
                         value[value.Length - 1] == '/' && Regex.IsMatch(value[value.Length - 2].ToString(), @"[0-9]+"))
                    {
                        value = value.Remove(value.Length - 1);
                    }

                    _IP = value;

                    NotifyPropertyChanged(nameof(IP));
                }
            }

            private string _Password;

            public string Password
            {
                get => _Password;
                set
                {
                    _Password = value;

                    NotifyPropertyChanged(nameof(Password));
                }
            }

            private ICommand _Add;

            public ICommand Add
            {
                get
                {
                    return _Add ?? (_Add = new RelayCommand(Execute =>
                    {
                        if (Dictionary)
                        {
                            if (!string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(IP))
                            {
                                Collection.Add(new IList(Key, IP, Password));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Key))
                            {
                                Collection.Add(new IList(Key));
                            }
                        }

                        Key = "";
                        IP = "";
                        Password = "";
                    }));
                }
            }

            public class IList
            {
                public string Key { get; set; }
                public string IP { get; set; }
                public string Password { get; set; }

                public IList(string Key)
                {
                    this.Key = Key;
                }

                public IList(string Key, string IP, string Password)
                {
                    this.Key = Key;
                    this.IP = IP;
                    this.Password = Password;
                }
            }

            private ObservableCollection<IList> _Collection = new ObservableCollection<IList>();

            public ObservableCollection<IList> Collection
            {
                get => _Collection;
                set
                {
                    _Collection = value;

                    NotifyPropertyChanged(nameof(Collection));
                    NotifyPropertyChanged(nameof(Visibility));
                }
            }

            public bool Visibility
            {
                get => Collection.Count > 0;
            }

            public void Update()
            {
                NotifyPropertyChanged(nameof(Visibility));
            }

            private bool _Dictionary;

            public bool Dictionary
            {
                get => _Dictionary;
                set
                {
                    _Dictionary = value;

                    NotifyPropertyChanged(nameof(Dictionary));
                    NotifyPropertyChanged(nameof(Watermark));
                }
            }

            public string Watermark
            {
                get => Dictionary
                    ? "Steam ID"
                    : "App ID";
            }


            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public readonly IAuto Auto = new IAuto();

        public Add(object X)
        {
            InitializeComponent();

            Auto.Key = "";
            Auto.IP = "";
            Auto.Password = "";

            Auto.Collection.Clear();
            Auto.Dictionary = false;

            var Type = X.GetType();

            if (Type == typeof(List<uint>)) // White List
            {
                var List = (List<uint>)X;

                if (List != null && List.Count > 0)
                {
                    Auto.Collection = new ObservableCollection<IAuto.IList>(List.Select(x => new IAuto.IList(x.ToString())));
                }
            }
            else if (Type == typeof(Dictionary<string, IConfig.IASF>)) // Account List
            {
                var Dictionary = (Dictionary<string, IConfig.IASF>)X;

                if (Dictionary != null && Dictionary.Count > 0)
                {
                    Auto.Collection = new ObservableCollection<IAuto.IList>(Dictionary.Select(x => new IAuto.IList(x.Key, x.Value.IP, x.Value.Password)));
                }

                Auto.Dictionary = true;
            }

            Auto.Update();

            Auto.Collection.CollectionChanged += List_CollectionChanged;

            DataContext = Auto;
        }

        private void Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var Button = sender as Button;

            if (Button == null || !(Button.DataContext is IAuto.IList X)) return;

            Auto.Collection.Remove(X);
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Auto.Update();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}
