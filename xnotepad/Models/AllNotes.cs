using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xnotepad.Common;

namespace xnotepad.Models;

internal class AllNotes
{
    public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
    public AllNotes() => LoadNotes();

    public void LoadNotes()
    {
        Notes.Clear();
        string appDataPath = FileSystem.AppDataDirectory;
        Trace.WriteLine($"app directory:{appDataPath}");

        // Use Linq extensions to load the *.notes.txt files.
        IEnumerable<Note> notes = Directory
            // select the filename
            .EnumerateFiles(appDataPath, $"*.{Constants.Version}.txt")
            // Each file name is used to create a new Note
            .Select(filename => new Note()
            {
                Filename = filename,
                Text = File.ReadAllText(filename),
                Date = File.GetLastWriteTime(filename),
                Brief = convertBrief(File.ReadAllText(filename))
            })
            // With the final collection of notes, order them by date
            .OrderBy(note => note.Date);

        // Add each note into the ObservableCollection
        foreach (Note note in notes) { 
            Notes.Add(note);
        }
    }

    private string convertBrief(string text)
    {
        string rt = "空";
        const int MIN_LENGTH = 28;

        if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
        {
            text = text.Trim();
            foreach (var myString in text.Split(new string[] { Environment.NewLine, "\r\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (myString?.Trim().Length > 0)
                {
                    var min = Math.Min(MIN_LENGTH, myString.Trim().Length);
                    return myString.Trim().Substring(0, min);
                }
            }
        }

        return rt;
    }
}

