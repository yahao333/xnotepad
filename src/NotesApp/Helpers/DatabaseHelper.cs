using System.Data.SQLite;
using System.IO;

namespace NotesApp.Helpers
{
    public static class DatabaseHelper
    {
        private const string DatabaseFileName = "data.db";
        private const string ConnectionString = "Data Source=data.db;Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFileName))
            {
                SQLiteConnection.CreateFile(DatabaseFileName);
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string createNotesTableQuery = @"
                        CREATE TABLE Notes (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            Content TEXT NOT NULL,
                            CreatedDate TEXT NOT NULL,
                            ModifiedDate TEXT NOT NULL
                        )";
                    string createTagsTableQuery = @"
                        CREATE TABLE Tags (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL
                        )";

                    string createNoteTagsTableQuery = @"
                        CREATE TABLE NoteTags (
                            NoteID INTEGER,
                            TagID INTEGER,
                            PRIMARY KEY (NoteID, TagID),
                            FOREIGN KEY (NoteID) REFERENCES Notes(ID),
                            FOREIGN KEY (TagID) REFERENCES Tags(ID)
                        )";
                    using (var command = new SQLiteCommand(createNotesTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (var command = new SQLiteCommand(createTagsTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (var command = new SQLiteCommand(createNoteTagsTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
