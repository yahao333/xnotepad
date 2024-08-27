using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NotesApp.ViewModels
{
    public class TagViewModel : INotifyPropertyChanged
    {
        private string _text1, _text2;
        public Geometry IconGeometry { get; set; }
        public string Text1
        {
            get => _text1;
            set
            {
                _text1 = value;
                OnPropertyChanged(nameof(Text1));
            }
        }
        public string Text2
        {
            get => _text2;
            set
            {
                _text2 = value;
                OnPropertyChanged(nameof(Text2));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
