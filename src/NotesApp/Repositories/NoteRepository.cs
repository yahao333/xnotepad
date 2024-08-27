using NotesApp.Helpers;
using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace NotesApp.Repositories
{
    public class NoteRepository
    {
        // 创建多对多关系表
        public class NoteTagMapping
        {
            public int NoteID { get; set; }
            public int TagID { get; set; }
        }


        public int Add(Note note)
        {
            int newId = -1;
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Start a transaction to ensure the insertion and retrieval of the ID are atomic
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert the note into the database
                        string query = "INSERT INTO Notes (Title, Content, CreatedDate, ModifiedDate) VALUES (@Title, @Content, @CreatedDate, @ModifiedDate)";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Title", note.Title);
                            command.Parameters.AddWithValue("@Content", note.Content);
                            command.Parameters.AddWithValue("@CreatedDate", note.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.Parameters.AddWithValue("@ModifiedDate", note.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                        }

                        // Retrieve the ID of the newly inserted record
                        string selectQuery = "SELECT last_insert_rowid()";
                        using (var command = new SQLiteCommand(selectQuery, connection))
                        {
                            object result = command.ExecuteScalar();
                            if (result != null && int.TryParse(result.ToString(), out int id))
                            {
                                newId = id;
                            }
                        }

                        transaction.Commit(); // Commit the transaction if everything is successful
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return newId;
        }

        public void Update(Note note)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Notes SET Title = @Title, Content = @Content, ModifiedDate = @ModifiedDate WHERE ID = @ID";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", note.ID);
                    command.Parameters.AddWithValue("@Title", note.Title);
                    command.Parameters.AddWithValue("@Content", note.Content);
                    command.Parameters.AddWithValue("@ModifiedDate", note.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Notes WHERE ID = @ID";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }

                string queryMap = "DELETE FROM NoteTags WHERE NoteID = @NoteID";
                using (var command = new SQLiteCommand(queryMap, connection))
                {
                    command.Parameters.AddWithValue("@NoteID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Note> GetAll()
        {
            var notes = new List<Note>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Notes";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = Convert.ToInt32(reader["ID"]);

                            ObservableCollection<NoteTag> tags = new ObservableCollection<NoteTag>();

                            string queryTags = $"SELECT Tags.ID, Tags.Name FROM NoteTags JOIN Tags ON NoteTags.TagID=Tags.ID WHERE NoteTags.NoteID={id}";
                            using (var commandTags = new SQLiteCommand(queryTags, connection))
                            {
                                using (var readerTags = commandTags.ExecuteReader())
                                {
                                    while (readerTags.Read())
                                    {
                                        var tagId = Convert.ToInt32(readerTags["ID"]);
                                        var tagName = readerTags["Name"].ToString();
                                        var noteTag = new NoteTag { ID = tagId, Name = tagName };
                                        tags.Add(noteTag);
                                    }
                                }
                            }
                            notes.Add(new Note
                            {
                                ID = id,
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                                ModifiedDate = DateTime.Parse(reader["ModifiedDate"].ToString()),
                                Tags = tags
                            });
                        }
                    }
                }
            }
            return notes;
        }
        private string ConvertFlowDocumentToString(FlowDocument document)
        {
            using (var memoryStream = new MemoryStream())
            {
                var textRange = new TextRange(document.ContentStart, document.ContentEnd);
                textRange.Save(memoryStream, DataFormats.Xaml);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        private FlowDocument ConvertStringToFlowDocument(string xamlString)
        {
            var flowDocument = new FlowDocument();
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xamlString)))
            {
                var textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                textRange.Load(memoryStream, DataFormats.Xaml);
            }
            return flowDocument;
        }

        public void AddTag(Tag tag)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Tags (Name) VALUES (@Name)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", tag.Name);
                    command.ExecuteNonQuery();
                    tag.ID = (int)connection.LastInsertRowId;
                }
            }
        }

        public void DeleteTag(Tag tag)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new SQLiteCommand("DELETE FROM NoteTags WHERE TagId = @TagID", connection);
                command.Parameters.AddWithValue("@TagID", tag.ID);
                command.ExecuteNonQuery();

                command = new SQLiteCommand("DELETE FROM Tags WHERE ID = @Id", connection);
                command.Parameters.AddWithValue("@Id", tag.ID);
                command.ExecuteNonQuery();
            }
        }
        public void DeleteTag(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        using (var command = new SQLiteCommand("DELETE FROM NoteTags WHERE TagId = @TagID", connection))
                        {
                            command.Parameters.AddWithValue("@TagID", id);
                            command.ExecuteNonQuery();
                        }

                        using (var command = new SQLiteCommand("DELETE FROM Tags WHERE ID = @Id", connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<Tag> GetTags()
        {
            var tags = new List<Tag>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Tags";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(new Tag
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            return tags;
        }

        public void AddNoteTag(int noteId, int tagId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO NoteTags (NoteID, TagID) VALUES (@NoteID, @TagID)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NoteID", noteId);
                    command.Parameters.AddWithValue("@TagID", tagId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Tag> GetNoteTags(int noteId)
        {
            var tags = new List<Tag>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                SELECT Tags.ID, Tags.Name
                FROM Tags
                INNER JOIN NoteTags ON Tags.ID = NoteTags.TagID
                WHERE NoteTags.NoteID = @NoteID";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NoteID", noteId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(new Tag
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            return tags;
        }

        public List<Note> SearchNotes(int tagId)
        {
            List<Note> notes = new List<Note>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string queryNoteIds = @"SELECT NoteID FROM NoteTags WHERE TagID=@TagID";
                using (var commandNoteIds = new SQLiteCommand(queryNoteIds, connection))
                {
                    commandNoteIds.Parameters.AddWithValue("@TagID", tagId);
                    using (var readerNoteIds = commandNoteIds.ExecuteReader())
                    {
                        while (readerNoteIds.Read())
                        {
                            var noteId = Convert.ToInt32(readerNoteIds["NoteID"]);
                            Trace.WriteLine($"noteId={noteId}");

                            string queryNotes = @"SELECT * FROM Notes WHERE ID=@ID";
                            using (var command = new SQLiteCommand(queryNotes, connection))
                            {
                                command.Parameters.AddWithValue("@ID", noteId);
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var id = Convert.ToInt32(reader["ID"]);
                                        var title = reader["Title"].ToString();
                                        var content = reader["Content"].ToString();
                                        var createdDate = reader["CreatedDate"].ToString();
                                        var modifiedDate = reader["ModifiedDate"].ToString();

                                        Trace.WriteLine($"Row: id={id} title={title} create={createdDate} modified={modifiedDate} content={content}");
                                        Note note = new Note { ID = id, Title = title, CreatedDate = DateTime.Parse(createdDate), ModifiedDate = DateTime.Parse(modifiedDate), Content = content };
                                        notes.Add(note);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return notes;
        }

        public List<Folder> GetAllFolders()
        {
            List<Folder> folders = new List<Folder>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT ID, Name FROM Tags";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tagId = Convert.ToInt32(reader["ID"]);
                            var name = reader["Name"].ToString();
                            int count = 0;

                            string queryCount = @"SELECT COUNT(*) FROM NoteTags WHERE TagID=@TagID";
                            using (var cmdCount = new SQLiteCommand(queryCount, connection))
                            {
                                cmdCount.Parameters.AddWithValue("@TagID", tagId);
                                using (var readerCount = cmdCount.ExecuteReader())
                                {
                                    if (readerCount.Read())
                                    {
                                        count = readerCount.GetInt32(0);
                                        Trace.WriteLine($"count={count}");
                                    }
                                }
                            }
                            folders.Add(new Folder { Id = tagId, Count = count, Name = name });
                        }
                    }
                }
            }
            return folders;
        }

        public void SaveCards(List<CardItem> cardItems)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cardItem in cardItems)
                        {
                            if (cardItem.IsContentModified)
                            {
                                string sql = "UPDATE Notes SET Content=@Content, ModifiedDate=CURRENT_TIMESTAMP WHERE ID=@ID";
                                using (var command = new SQLiteCommand(sql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Content", cardItem.Content);
                                    command.Parameters.AddWithValue("@ID", cardItem.Id);

                                    int rowAffected = command.ExecuteNonQuery();

                                    //if (rowAffected == 0)
                                    //{
                                    //    // 如果没有更新任何行，可能需要插入新记录
                                    //    sql = "INSERT INTO Notes (Content, CreatedDate, ModifiedDate) VALUES (@Content, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                                    //    command.CommandText = sql;
                                    //    command.Parameters.Clear();
                                    //    command.Parameters.AddWithValue("@Content", cardItem.Content);
                                    //    command.ExecuteNonQuery();
                                    //}
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // 在实际应用中，您可能想要记录错误或重新抛出异常
                        Console.WriteLine($"Error saving cards: {ex.Message}");
                    }
                }
            }
        }

        public void UpdateTag(Tag tag)
        {
            if (tag == null || tag.ID <= 0)
            {
                throw new ArgumentException("Invalid tag provided. Ensure the tag is not null and has a valid ID.");
            }

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update existing tag
                        string updateQuery = "UPDATE Tags SET Name = @Name WHERE ID = @ID";
                        using (var command = new SQLiteCommand(updateQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ID", tag.ID);
                            command.Parameters.AddWithValue("@Name", tag.Name);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new InvalidOperationException("Tag update failed. No rows affected.");
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Handle exceptions as needed
                        Console.WriteLine($"Error updating tag: {ex.Message}");
                        throw;
                    }
                }
            }
        }

    }
}
