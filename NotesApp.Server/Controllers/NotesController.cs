using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Server.Model;
using Npgsql;

namespace NotesApp.Server.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class NotesController : Controller
    {
        // POST: New note 
        [HttpPost("/notes/Create")]
        public async Task<ActionResult> Create(NoteDTO note)
        {

            var connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; Username={Environment.GetEnvironmentVariable("Username")};Password={Environment.GetEnvironmentVariable("Password")};Database=" + $"{Environment.GetEnvironmentVariable("Database")};";
            await using var connection = Npgsql.NpgsqlDataSource.Create(connectionString);

            try
            {
                if (note == null)
                {
                    return BadRequest("Something went wrong!");
                }

                var sql = "INSERT INTO notes (notetitle, notebody) VALUES (@Title, @BodyText)";

                using (var command = connection.CreateCommand($"{sql}"))
                {
                    command.Parameters.AddWithValue("@Title", note.Title);
                    command.Parameters.AddWithValue("@BodyText", note.BodyText);
                    await command.ExecuteNonQueryAsync();
                }

                return Ok(note);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }

        //DELETE : Delete a note by ID
        [HttpDelete("/notes/{id}")]
        public async Task<ActionResult> DeleteNote(int id)
        {
            var connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; Username={Environment.GetEnvironmentVariable("Username")};Password={Environment.GetEnvironmentVariable("Password")};Database={Environment.GetEnvironmentVariable("Database")};";

            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();

                var sql = "DELETE FROM notes WHERE noteid = @Id";

                await using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }

                return StatusCode(200, $"Note with id {id} has been deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET : Get all notes
        [HttpGet("/notes/getAll")]
        public async Task<ActionResult> GetNotes()
        {
            var notes = new List<NoteDTO>();
            var connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; Username={Environment.GetEnvironmentVariable("Username")};Password={Environment.GetEnvironmentVariable("Password")};Database={Environment.GetEnvironmentVariable("Database")};";
            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();
                var sql = "SELECT noteid, notetitle, notebody FROM notes";
                await using (var command = new NpgsqlCommand(sql, connection))
                {
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var noteId = reader.GetInt32(reader.GetOrdinal("noteid"));
                            var notetitle = reader.GetString(reader.GetOrdinal("notetitle"));
                            var notebody = reader.GetString(reader.GetOrdinal("notebody"));

                            var note = new NoteDTO
                            {
                                Id = noteId,
                                Title = notetitle,
                                BodyText = notebody
                            };

                            notes.Add(note);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(notes);
        }

        //GET : Only one NOTE
        [HttpGet("/notes/{id}")]
        public async Task<ActionResult<NoteDTO>> GetNoteById(int id)
        {
            var connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; Username={Environment.GetEnvironmentVariable("Username")};Password={Environment.GetEnvironmentVariable("Password")};Database={Environment.GetEnvironmentVariable("Database")};";
            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();

                var sql = "SELECT noteid, notetitle, notebody FROM notes WHERE noteid = @Id";

                await using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var noteId = reader.GetInt32(reader.GetOrdinal("noteid"));
                    var notetitle = reader.GetString(reader.GetOrdinal("notetitle"));
                    var notebody = reader.GetString(reader.GetOrdinal("notebody"));

                    var note = new NoteDTO
                    {
                        Id = noteId,
                        Title = notetitle,
                        BodyText = notebody
                    };

                    return Ok(note);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //UPDATE: Update a note by ID
        [HttpPut("/notes/update/{id}")]
        public async Task<ActionResult> UpdateNote(int id, NoteDTO note)
        {
            var connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; Username={Environment.GetEnvironmentVariable("Username")};Password={Environment.GetEnvironmentVariable("Password")};Database={Environment.GetEnvironmentVariable("Database")};";

            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();

                var sql = "UPDATE notes SET notetitle = @notetitle, notebody = @notebody WHERE noteid = @id";

                await using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@notetitle", note.Title);
                    command.Parameters.AddWithValue("@notebody", note.BodyText);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok($"The note with id {id} has been updated!");
        }


    }
}
