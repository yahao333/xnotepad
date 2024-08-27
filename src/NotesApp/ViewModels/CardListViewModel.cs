using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NotesApp.Models;
using NotesApp.Repositories;
using System.Windows.Input;
using NotesApp.Commands;

namespace NotesApp.ViewModels
{
    public delegate void delegateRequestReturnIndexWindow();
    public class CardListViewModel : INotifyPropertyChanged
    {
        private NoteRepository _noteRepository;
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private Folder _folder;
        private ObservableCollection<CardItem> _cardItems;
        public ObservableCollection<CardItem> CardItems
        {
            get { return _cardItems; }
            set
            {
                _cardItems = value;
                OnPropertyChanged(nameof(CardItems));
            }
        }

        public delegateRequestReturnIndexWindow RequestReturnIndexWindowHandle;

        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand NewCardCommand { get; }
        public ICommand ReturnCommand { get; }
        #endregion

        public CardListViewModel(Folder folder)
        {
            _folder = folder;
            //CardItems = new ObservableCollection<CardItem>
            //{
            //    new CardItem { Type= CardType.Note, Title = "# Hello!", Description = "*SideNotes* is a note-taking app...", Timestamp = "18/05/2019 at 11:54" },
            //    new CardItem { Type=CardType.Color, Title = "# Colors", Description = "Here are some colors:", Colors = new ObservableCollection<string> { "Red", "Yellow", "Green", "Blue", "Gray" } },
            //    new CardItem { Type=CardType.Task, Title = "# Tasks", Tasks = new ObservableCollection<string> { "make coffee", "check that project", "pay the bills" } }
            //};

            Title = folder.Name;

            CardItems = new ObservableCollection<CardItem>();

            _noteRepository = new NoteRepository();
            var notes = _noteRepository.SearchNotes(folder.Id);
            var sortedNotes = notes.OrderByDescending(n => n.CreatedDate).ToList();
            foreach (var item in sortedNotes)
            {
                CardItems.Add(new CardItem(RemoveCardItem) { Id = item.ID, Type = CardType.Note, Title = item.Title, Content = item.Content, IsContentModified = false });
            }

            SaveCommand = new RelayCommand(saveCards, CanSave);
            NewCardCommand = new RelayCommand(newCard, CanNew);
            ReturnCommand = new RelayCommand(returnIndexWindow, CanReturnIndexWindow);
        }

        private void returnIndexWindow(object obj)
        {
            RequestReturnIndexWindowHandle?.Invoke();
        }

        private bool CanReturnIndexWindow(object arg)
        {
            return true;
        }

        private void newCard(object obj)
        {
            AddCard(new CardItem(RemoveCardItem) { Title = "New Title", Content = "New Card" });
        }

        private bool CanNew(object arg)
        {
            return true;
        }

        private void RemoveCardItem(CardItem cardItem)
        {
            CardItems.Remove(cardItem);

            _noteRepository.Delete(cardItem.Id);
        }

        private void saveCards(object obj)
        {
            Save();
        }

        private bool CanSave(object arg)
        {
            foreach (var cardItem in CardItems)
            {
                if (cardItem.IsContentModified)
                {
                    return true;
                }
            }

            return false;
        }

        public void Save()
        {
            _noteRepository.SaveCards(new List<CardItem>(CardItems));
        }

        public void AddCard(CardItem cardItem)
        {
            Note note = new Note();
            note.CreatedDate = DateTime.Now;
            note.ModifiedDate = DateTime.Now;
            note.Content = cardItem.Content;
            note.Title = cardItem.Title;

            int id = _noteRepository.Add(note);
            note.ID = id;

            _noteRepository.AddNoteTag(id, _folder.Id);

            CardItem newCardItem = new CardItem(RemoveCardItem) { 
                Id = note.ID, Type = CardType.Note, Content = note.Content, IsContentModified = false,
                CreatedDate = note.CreatedDate, ModifiedDate = note.ModifiedDate 
            };

            var cardItems = new List<Note>();
            foreach (var item in CardItems)
            {
                cardItems.Add(new Note
                {
                    ID = item.Id,
                    Content = item.Content,
                    Title = item.Title,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate
                });
            }
            cardItems.Add(note);

            var sortedNotes = cardItems.OrderByDescending(n => n.CreatedDate).ToList();
            CardItems.Clear();
            foreach (var item in sortedNotes)
            {
                CardItems.Add(new CardItem(RemoveCardItem) { Id = item.ID, Type = CardType.Note, Title = item.Title, Content = item.Content, IsContentModified = false });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
