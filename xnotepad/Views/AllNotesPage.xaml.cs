using xnotepad.Common;

namespace xnotepad.Views;

public partial class AllNotesPage : ContentPage
{
	public AllNotesPage()
	{
		InitializeComponent();

        BindingContext = new Models.AllNotes();

        this.Title = $"{Constants.AppName} {Constants.Version}";
    }
    protected override void OnAppearing()
    {
        ((Models.AllNotes)BindingContext).LoadNotes();
    }

    private async void notesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count != 0)
        {
            var note = (Models.Note)e.CurrentSelection[0];

            await Shell.Current.GoToAsync($"{nameof(NotePage)}?{nameof(NotePage.ItemId)}={note.Filename}");

            notesCollection.SelectedItem = null;
        }
    }

    private async void Nagvi_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NotePage));
    }
}