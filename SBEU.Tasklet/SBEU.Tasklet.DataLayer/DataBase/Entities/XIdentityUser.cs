
using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations.Schema;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XIdentityUser : IdentityUser, IEntity
    {
        public XIdentityUser()
        {
            Chats??= new HashSet<Chat>();
            Confirmations ??= new HashSet<XIdentityUserConfirm>();
            Notes ??= new HashSet<Note>();
            Tasks ??= new HashSet<XTask>();
            Tables ??= new HashSet<XTable>();
            AuthoredTasks ??= new HashSet<XTask>();
        }

        public string? PushToken { get; set; }
        public bool IsPushOn { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<XIdentityUserConfirm> Confirmations { get; set; }
        public virtual ICollection<XTable> Tables { get; set; }
        public virtual ICollection<XTask> Tasks { get; set; }
        public virtual ICollection<XTask> AuthoredTasks { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }
}
