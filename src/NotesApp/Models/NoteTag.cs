using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Models
{
    public class NoteTag : INotifyPropertyChanged
    {
        private int id;
        private string name;
        public int ID {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        public string Name {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
