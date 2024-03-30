using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Server.Model
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Title { get; set; }
        public string BodyText {  get; set; }
    }

    public class NoteDTO
    { 
        public long Id { get; set; }
        public string Title { get; set; }
        public string BodyText { get; set; }
    }
}
