using NotesApp.Commands;
using NotesApp.Helpers;
using NotesApp.Models;
using NotesApp.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public delegate void delegateRequestCardList(Folder folder);
    public class IndexViewModel : BaseViewModel
    {
        #region Properties
        public ObservableCollection<Folder> Folders { get; set; }
        private NoteRepository _noteRepository;
        public delegateRequestCardList cardListRequest;
        private Window _window;

        public Folder SelectedFolder { get => GetProperty<Folder>(); set => SetProperty(value); }
        #endregion
        #region ctor
        public IndexViewModel(Window window)
        {
            this._window = window;
            init();
        }
        #endregion
        #region Methods
        private void init()
        {
            _noteRepository = new NoteRepository();
            DatabaseHelper.InitializeDatabase();

            ItemDoubleClickCommand = new RelayCommand(OnItemDoubleClick);
            RenameCommand = new RelayCommand(RenameClicked);
            EditCommand = new RelayCommand(EditClicked, CanEdit);
            SaveTagCommand = new RelayCommand(SaveTagClicked, CanSave);
            FilterCommand = new RelayCommand(FilterClicked);
            AddTagCommand = new RelayCommand(AddTagClicked);
            DeleteCommand = new RelayCommand(DeleteClicked, CanDelete);

            Folders = new ObservableCollection<Folder>(_noteRepository.GetAllFolders());
            foreach (var folder in Folders)
            {
                folder.SetOwner(this);
            }

        }

        private bool CanDelete(object arg)
        {
            return SelectedFolder != null;
        }

        private void DeleteClicked(object obj)
        {
            Trace.WriteLine("delete");
            if (null != obj && obj is Folder selectedFolder)
            {
                Folders.Remove(selectedFolder);
                SelectedFolder = null;

                // db
                _noteRepository.DeleteTag(selectedFolder.Id);
            }
        }

        private void AddTagClicked(object obj)
        {
            var newTag = new Tag { Name = "New Tag" };
            _noteRepository.AddTag(newTag);
            Folder folder = new Folder { Id = newTag.ID, Name = newTag.Name, IsEditing = true };
            folder.SetOwner(this);
            Folders.Add(folder);
        }

        private void FilterClicked(object obj)
        {
            MessageBox.Show("未实现");
        }

        private bool CanSave(object arg)
        {
            if(SelectedFolder != null)
            {
                return veridateTagName(SelectedFolder);
            }
            return false;
        }

        private bool CanEdit(object arg)
        {
            return SelectedFolder != null;
        }

        public bool veridateTagName(Folder folder)
        {
            foreach (var f in Folders)
            {
                if(f == folder)
                {
                    continue;
                }

                if(f.Name == folder.Name)
                {
                    return false;
                }
            }

            return true;
        }
        private void SaveTagClicked(object obj)
        {
            if (obj is Folder folder)
            {
                if (!veridateTagName(folder))
                {
                    MessageBox.Show($"名称[{folder.Name}]已存在");
                    folder.Name = folder.OriginName;
                    return;
                }

                Tag tag = new Tag { ID = folder.Id, Name = folder.Name };
                _noteRepository.UpdateTag(tag);
            }
        }

        private void EditClicked(object obj)
        {
            if (null != obj && obj is Folder selectedFolder)
            {
                foreach (var folder in Folders)
                {
                    if (folder.Id == selectedFolder.Id)
                    {
                        Trace.WriteLine($"get it, id={folder.Id} name={folder.Name}");
                        folder.OriginName = folder.Name;
                        folder.IsEditing = true;
                        break;
                    }
                }
            }
        }

        private void RenameClicked(object obj)
        {
            MessageBox.Show("未实现");
        }

        public void OnItemDoubleClick(object parameter)
        {
            EditCommand?.Execute(parameter);
        }

        public void OpenSelecteFolder(object parameter)
        {
            if (parameter is Folder selectedItem)
            {
                Trace.WriteLine(selectedItem);
                cardListRequest?.Invoke(selectedItem);
                this._window.Close();
            }
        }
        #endregion

        #region Commands
        public ICommand ItemDoubleClickCommand { get; set; }
        public ICommand RenameCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SaveTagCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand AddTagCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        #endregion

    }
}
