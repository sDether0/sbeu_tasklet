﻿
using Microsoft.AspNetCore.Identity;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XIdentityUser : IdentityUser, IIdEntity
    {
        public string? PushToken { get; set; }
        public bool IsPushOn { get; set; }
        public bool IsMailNotify { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<XIdentityUserConfirm> Confirmations { get; set; }
        public virtual ICollection<XTable> Tables { get; set; }
        public virtual ICollection<XTask> Tasks { get; set; }
        public virtual ICollection<XTask> AuthoredTasks { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }
}
