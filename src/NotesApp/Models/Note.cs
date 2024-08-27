using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NotesApp.Models
{
    public class Note : INotifyPropertyChanged
    {
        private int id;
        private string title, content;
        private DateTime createdDate, modifiedDate;
        private ObservableCollection<NoteTag> tags;
        public int ID
        {
            get => id; set
            {
                id = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        public string Title {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Content {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        public DateTime CreatedDate {
            get => createdDate;
            set
            {
                createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }
        public DateTime ModifiedDate {
            get => modifiedDate;
            set
            {
                modifiedDate = value;
                OnPropertyChanged(nameof(ModifiedDate));
            }
        }

        public ObservableCollection<NoteTag> Tags {
            get => tags;
            set
            {
                tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
