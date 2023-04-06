
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;
using SBEU.Tasklet.Models.Enums;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XTask : DeletableEntity
    {
        public XTask()
        {
            //Contents ??= new HashSet<XContent>();
            //Notes ??= new HashSet<Note>();
            //History ??= new HashSet<XHistory>();
            //Links ??= new List<string>();
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        //[DefaultValue(0)]
        public long StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual TaskProgress Status { get; set; }
        public bool Hidden { get; set; }
        public List<string> Links { get; set; }
        public uint Price { get; set; }
        public virtual XTable Table { get; set; }
        [Required,NotNull]
        public virtual XIdentityUser Author { get; set; }
        [Required,NotNull]
        public virtual XIdentityUser Executor { get; set; }
        public virtual ICollection<XContent> Contents { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<XHistory> History { get; set; }

    }
}
