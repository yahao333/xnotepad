using NotesApp.Commands;
using NotesApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

public class CardItem : BaseViewModel
{
    public int Id { get => GetProperty<int>(); set => SetProperty(value); }
    public CardType Type { get => GetProperty<CardType>(); set => SetProperty(value); }
    public string Title { get => GetProperty<string>(); set => SetProperty(value); }
    public bool IsContentModified { get => GetProperty<bool>(); set => SetProperty(value); }
    public DateTime CreatedDate { get => GetProperty<DateTime>(); set => SetProperty(value); }
    public DateTime ModifiedDate { get => GetProperty<DateTime>(); set => SetProperty(value); }
    public string Content { 
        get => GetProperty<string>();
        set
        {
            if (SetProperty(value))
            {
                IsContentModified = true;
            }
        }
    }
    public string Timestamp { get; set; }
    public ObservableCollection<string> Colors { get => GetProperty<ObservableCollection<string>>(); set => SetProperty(value); } // For color cards
    public ObservableCollection<string> Tasks { get => GetProperty<ObservableCollection<string>>(); set => SetProperty(value); } // For task cards

    private readonly Action<CardItem> _removeAction;
    #region ctor
    public CardItem(Action<CardItem> removeAction)
    {
        _removeAction = removeAction;
        RemoveCommand = new RelayCommand(ExecuteRemove, canRemove);
    }

    private bool canRemove(object arg)
    {
        return true;
    }
    #endregion
    #region Commands
    public ICommand RemoveCommand { get; }
    #endregion
    #region Methods
    private void ExecuteRemove(object obj)
    {
        _removeAction(this);
    }

    #endregion
}

