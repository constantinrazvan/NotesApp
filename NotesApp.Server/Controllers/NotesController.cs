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
        [HttpPost("/Create")]
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
        [HttpDelete("/{id}")]
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
        [HttpGet("/getAll")]
        public async Task<ActionResult> GetNotes()
        {
            var notes = new List<NoteDTO>();
            var connectionString = $"Host={Environment.GetEnvironmentVariable("Host")}; Username={Environment.GetEnvironmentVariable("Username")};Password={Environment.GetEnvironmentVariable("Password")};Database={Environment.GetEnvironmentVariable("Database")};";
            await using var connection = new NpgsqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();
                var sql = "SELECT * FROM notes";
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
                                Title = notetitle,
                                BodyText = notebody
                            };

                            notes.Add(note);

                        }
                    }
                }

            } catch(Exception ex)
            {

            }

            return Ok(notes.ToList());
        }

    }
}
