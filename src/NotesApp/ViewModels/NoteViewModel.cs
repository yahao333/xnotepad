using NotesApp.Commands;
using NotesApp.Models;
using NotesApp.Repositories;
using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private Note _note;
        private Timer _autoSaveTimer;

        public Note Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public NoteViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            //_autoSaveTimer = new Timer(5000); // Auto-save every 5 seconds
            //_autoSaveTimer.Elapsed += AutoSave;
            //_autoSaveTimer.Start();
        }
        private void AutoSave(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => SaveImpl());
        }
        private void SaveImpl()
        {
            // Implement save logic here, e.g., assign an ID if new, update ModifiedDate
            if (Note.ID == 0)
            {
                Note.ID = new Random().Next(1, 10000);  // Assign a new ID
                Note.CreatedDate = DateTime.Now;
            }
            Note.ModifiedDate = DateTime.Now;

            // Save the note to the database
            var noteRepository = new NoteRepository();
            noteRepository.Update(Note);

            //if (Note.ID == 0)
            //{
            //    noteRepository.Add(Note);
            //}
            //else
            //{
            //    noteRepository.Update(Note);
            //}
        }
        private void Save(object obj)
        {
            SaveImpl();

            // Close the window
            Application.Current.Windows[1]?.Close();
        }

        private void Cancel(object obj)
        {
            _autoSaveTimer.Stop();

            // Close the window
            Application.Current.Windows[1]?.Close();
        }

        public static NoteViewModel Of(Note note)
        {
            NoteViewModel nvm = new NoteViewModel();
            nvm.Note = note;

            return nvm;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
