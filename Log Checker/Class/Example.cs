using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.ComponentModel;

namespace LogChecker
{
    public class Example
    {
        public class ProgressBar : INotifyPropertyChanged, IDisposable
        {
            private int _Maximum;

            public int Maximum
            {
                get => _Maximum;
                set
                {
                    _Maximum = value;

                    NotifyPropertyChanged(nameof(Maximum));
                    NotifyPropertyChanged(nameof(Visibility));
                    NotifyPropertyChanged(nameof(Content));
                }
            }

            public bool Visibility
            {
                get => Maximum > 0;
            }

            private int _Value;

            public int Value
            {
                get => _Value;
                set
                {
                    _Value = value;

                    NotifyPropertyChanged(nameof(Value));
                    NotifyPropertyChanged(nameof(Content));
                }
            }

            public string Content
            {
                get
                {
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

                    TaskbarManager.Instance.SetProgressValue(Value, Maximum);

                    return $"{Value}/{Maximum} ({Math.Round((double)Value / Maximum * 100, 2)}%)";
                }
            }

            public void Dispose()
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
