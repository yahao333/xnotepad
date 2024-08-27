using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using NotesApp.Commands;
using NotesApp.Helpers;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Views;

namespace NotesApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public TagViewModel Item1 { get; set; } = new TagViewModel { IconGeometry = Geometry.Parse("M10,100 C40,10 65,10 95,100 S150,190 180,100"), Text1 = "Item 1", Text2 = "Editable Text 1" };
        private ObservableCollection<NoteViewModel> _notes;
        private NoteViewModel _selectedNote;
        private NoteRepository _noteRepository;

        private string _searchText;
        private ObservableCollection<NoteViewModel> _filteredNotes;

        private ObservableCollection<Tag> _tags;
        private Tag _selectedTag;

        public ObservableCollection<NoteViewModel> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }
        public NoteViewModel SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                _selectedNote = value;
                OnPropertyChanged(nameof(SelectedNote));
            }
        }
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterNotes();
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

        public ObservableCollection<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public Tag SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                _selectedTag = value;
                OnPropertyChanged(nameof(SelectedTag));
                FilterNotesByTag();
            }
        }
        public ICommand OpenCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SortByTitleCommand { get; }
        public ICommand SortByDateCommand { get; }
        public MainViewModel()
        {
            _noteRepository = new NoteRepository();
            DatabaseHelper.InitializeDatabase();
            var notesData = new ObservableCollection<Note>(_noteRepository.GetAll());
            Notes = new ObservableCollection<NoteViewModel>();
            foreach (var item in notesData)
            {
                Notes.Add(NoteViewModel.Of(item));
            }
            FilteredNotes = new ObservableCollection<NoteViewModel>(Notes);

            Tags = new ObservableCollection<Tag>(_noteRepository.GetTags());

            AddTagCommand = new RelayCommand(AddTag);
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, CanEditOrDelete);
            DeleteCommand = new RelayCommand(DeleteTag, CanEditOrDelete);
            OpenCommand = new RelayCommand(OpenTag, CanOpen);

            SortByTitleCommand = new RelayCommand(SortByTitle);
            SortByDateCommand = new RelayCommand(SortByDate);
        }
        private void AddTag(object obj)
        {
            if(obj is string tagText)
            {
                tagText = tagText.Trim();
                var newTag = new Tag { Name = tagText };
                _noteRepository.AddTag(newTag);
                Tags.Add(newTag);
            }
        }
        private void DeleteTag(object parameter)
        {
            if (SelectedTag != null && parameter is Tag tag)
            {
                Tags.Remove(tag);
                _noteRepository.DeleteTag(tag);
            }
        }
        private void OpenTag(object parameter)
        {
            if (SelectedTag != null && parameter is Tag tag)
            {
                Trace.WriteLine($"Open Tag:{tag.ID} {tag.Name}");
                OpenImpl(tag);
            }
        }
        private void FilterNotesByTag()
        {
            if (SelectedTag == null)
            {
                FilterNotes();
            }
            else
            {
                var taggedNotes = Notes.Where(note =>
                    note.Note.Tags.Any(tag => tag.ID == SelectedTag.ID));
                FilteredNotes = new ObservableCollection<NoteViewModel>(taggedNotes);
            }
        }
        private void SortByTitle(object obj)
        {
            Notes = new ObservableCollection<NoteViewModel>(Notes.OrderBy(n => n.Note.Title));
            FilterNotes();
        }

        private void SortByDate(object obj)
        {
            Notes = new ObservableCollection<NoteViewModel>(Notes.OrderBy(n => n.Note.ModifiedDate));
            FilterNotes();
        }
        private void FilterNotes()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredNotes = new ObservableCollection<NoteViewModel>(Notes);
            }
            else
            {
                FilteredNotes = new ObservableCollection<NoteViewModel>(Notes.Where(n => n.Note.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                                               n.Note.Content.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
            }
        }

        private void OpenImpl(Tag tag)
        {
            FilterNotesByTag();

            var noteListViewModel = new NoteListViewModel { 
                FilteredNotes = FilteredNotes, 
                Notes = FilteredNotes,
                TagName = tag.Name,
            };
            var noteListPage = new NoteListPage(noteListViewModel);
            noteListPage.ShowDialog();
        }
        private void Create(object obj)
        {
            var noteViewModel = new NoteViewModel { Note = new Note { CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now } };
            var notePage = new NotePage(noteViewModel);
            notePage.ShowDialog();

            if (noteViewModel.Note.ID != 0)
            {
                _noteRepository.Add(noteViewModel.Note);
                Notes.Add(noteViewModel);
            }
        }

        private void Edit(object obj)
        {
            if (SelectedNote == null) return;

            var noteViewModel = SelectedNote;
            var notePage = new NotePage(noteViewModel);
            notePage.ShowDialog();

            if (noteViewModel.Note.ID != 0)
            {
                _noteRepository.Update(noteViewModel.Note);
                var index = Notes.IndexOf(SelectedNote);
                Notes[index] = noteViewModel;
                //FilterNotes();
            }
        }

        private void Delete(object obj)
        {
            if (SelectedNote != null)
            {
                _noteRepository.Delete(SelectedNote.Note.ID);
                Notes.Remove(SelectedNote);
                SelectedNote = null;
            }
        }

        private bool CanEditOrDelete(object obj)
        {
            return SelectedTag != null;
        }
        private bool CanOpen(object obj)
        {
            return SelectedTag != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
