using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LogChecker
{
    public partial class Selection
    {
        #region Auto

        public class IList : INotifyPropertyChanged
        {
            private KeyValuePair<string, IConfig.IASF> _Pair;

            public KeyValuePair<string, IConfig.IASF> Pair
            {
                get => _Pair;
                set
                {
                    _Pair = value;

                    NotifyPropertyChanged(nameof(Pair));
                }
            }

            private string _Index;

            public string Index
            {
                get => _Index;
                set
                {
                    _Index = value;

                    NotifyPropertyChanged(nameof(Index));
                }
            }

            private bool _Checked = true;

            public bool Checked
            {
                get => _Checked;
                set
                {
                    _Checked = value;

                    NotifyPropertyChanged(nameof(Checked));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class IAuto : INotifyPropertyChanged
        {
            private List<IList> _List;

            public List<IList> List
            {
                get => _List;
                set
                {
                    _List = value;

                    NotifyPropertyChanged(nameof(List));
                }
            }

            private bool _Package;

            public bool Package
            {
                get => _Package;
                set
                {
                    _Package = value;

                    NotifyPropertyChanged(nameof(Package));
                }
            }

            private bool _Visibility;

            public bool Visibility
            {
                get => _Visibility;
                set
                {
                    _Visibility = value;

                    NotifyPropertyChanged(nameof(Visibility));
                }
            }

            private bool _Show = true;

            public bool Show
            {
                get => _Show;
                set
                {
                    _Show = value;

                    NotifyPropertyChanged(nameof(Show));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public readonly IAuto Auto = new IAuto();

        public Selection(Dictionary<KeyValuePair<string, IConfig.IASF>, int> Dictionary, bool Package)
        {
            InitializeComponent();

            Auto.List = Dictionary
                .Select(x => new IList { Pair = x.Key, Index = $"{x.Value:000}" })
                .ToList();

            if (Package)
            {
                Auto.Package = true;
                Auto.Visibility = true;
            }

            Auto.Show = true;

            DataContext = Auto;
        }

        private void Package_Click(object sender, RoutedEventArgs e)
        {
            Auto.Package = !Auto.Package;
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            Auto.Show = !Auto.Show;
        }

        private void Selected_Click(object sender, RoutedEventArgs e)
        {
            var ToggleButton = sender as ToggleButton;

            if (ToggleButton == null || !(ToggleButton.DataContext is IList X)) return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                foreach (var x in Auto.List)
                {
                    x.Checked = X.Checked;
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWindow = Application.Current.MainWindow;

            Left = MainWindow.Left + (MainWindow.Width - ActualWidth) / 2;
            Top = MainWindow.Top + (MainWindow.Height - ActualHeight) / 2;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}
