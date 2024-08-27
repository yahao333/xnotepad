using NotesApp.Commands;
using NotesApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.Models
{
    public class Folder : BaseViewModel
    {
        public int Id { get => GetProperty<int>(); set => SetProperty(value); }
        public string Name { get => GetProperty<string>(); set => SetProperty(value); }
        public int Count { get => GetProperty<int>(); set => SetProperty(value); }
        public bool IsEditing { get => GetProperty<bool>(); set => SetProperty(value); }
        public string OriginName { get => GetProperty<string>(); set => SetProperty(value); }

        private IndexViewModel indexViewModel;

        public ICommand OpenCommand { get; }
        public ICommand RenameCommand { get; }
        public ICommand ConfirmEditCommand { get; }
        public Folder()
        {
            OpenCommand = new RelayCommand(openFolder, canOpenFolder);
            RenameCommand = new RelayCommand(rename, canRename);
            ConfirmEditCommand = new RelayCommand(confirmEdit, canConfirm);
        }

        private void confirmEdit(object obj)
        {
            IsEditing = false;
        }

        private bool canConfirm(object arg)
        {
            return true;
        }

        private void rename(object obj)
        {
            IsEditing = true;
        }

        private bool canRename(object arg)
        {
            return true;
        }

        private void openFolder(object obj)
        {
            this.indexViewModel?.OpenSelecteFolder(this);
        }

        private bool canOpenFolder(object arg)
        {
            return true;
        }

        public void SetOwner(IndexViewModel indexViewModel)
        {
            this.indexViewModel = indexViewModel;
        }
    }
}
