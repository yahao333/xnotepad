using NotesApp.Commands;
using NotesApp.Models;
using NotesApp.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NoteListViewModel : INotifyPropertyChanged
    {
        private Note _selectedNote;
        private NoteRepository _noteRepository;

        private string _tagName;
        public string TagName
        {
            get => _tagName;
            set
            {
                _tagName = value;
                OnPropertyChanged(nameof(TagName));
            }
        }

        private ObservableCollection<NoteViewModel> _notes;
        private ObservableCollection<NoteViewModel> _filteredNotes;
        public ObservableCollection<NoteViewModel> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }
        public ObservableCollection<NoteViewModel> FilteredNotes
        {
            get { return _filteredNotes; }
            set
            {
                _filteredNotes = value;
                OnPropertyChanged(nameof(FilteredNotes));
            }
        }

        public ICommand CreateNoteCommand { get; }
        
        public NoteListViewModel()
        {
            CreateNoteCommand = new RelayCommand(CreateNote, CanCreateNote);
        }

        private bool CanCreateNote(object obj)
        {
            return true;
        }

        private void CreateNote(object arg)
        {
            Trace.WriteLine($"create note");
            var noteViewModel = new NoteViewModel { Note = new Note { CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now } };
            

            if (noteViewModel.Note.ID != 0)
            {
                _noteRepository.Add(noteViewModel.Note);
                Notes.Add(noteViewModel);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
