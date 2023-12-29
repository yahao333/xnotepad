using xnotepad.Common;

namespace xnotepad.Views;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public partial class NotePage : ContentPage
{
    public string ItemId
    {
        set { LoadNote(value); }
    }

    private void LoadNote(string fileName)
    {
        Models.Note noteModel = new Models.Note();
        noteModel.Filename = fileName;

        if (File.Exists(fileName))
        {
            noteModel.Text = File.ReadAllText(fileName);
        }

        BindingContext = noteModel;
    }

    public NotePage()
    {
        InitializeComponent();

        string appDataPath = FileSystem.AppDataDirectory;
        string randomFilename = $"{Path.GetRandomFileName()}.{Constants.Version}.txt";

        LoadNote(Path.Combine(appDataPath, randomFilename));

        this.Title = $"{Constants.AppName} {Constants.Version} - {randomFilename}";
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Models.Note note)
        {
            File.WriteAllText(note.Filename, note.Text);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Models.Note note)
        {
            if(File.Exists(note.Filename))
            {
                File.Delete(note.Filename);
            }
        }

        await Shell.Current.GoToAsync("..");
    }
}