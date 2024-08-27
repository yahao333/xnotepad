using NotesApp.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private Settings _settings;

        public Settings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get { return _isDarkMode; }
            set
            {
                _isDarkMode = value;
                OnPropertyChanged(nameof(IsDarkMode));
                ApplyTheme();
            }
        }

        private void ApplyTheme()
        {
            var dict = new ResourceDictionary();
            if (IsDarkMode)
            {
                dict.Source = new Uri("Resources/DarkTheme.xaml", UriKind.Relative);
            }
            else
            {
                dict.Source = new Uri("Resources/LightTheme.xaml", UriKind.Relative);
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public ICommand ApplyThemeCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
