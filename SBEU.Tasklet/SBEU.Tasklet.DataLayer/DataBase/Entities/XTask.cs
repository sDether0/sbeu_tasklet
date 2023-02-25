
using SBEU.Tasklet.Models.Enums;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XTask : IEntity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TaskProgress Status { get; set; }
        public bool Hidden { get; set; }
        public List<string> Links { get; set; }
        public uint Price { get; set; }
        public virtual XTable Table { get; set; }
        public virtual XIdentityUser Author { get; set; }
        public virtual XIdentityUser Executor { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        
    }
}
