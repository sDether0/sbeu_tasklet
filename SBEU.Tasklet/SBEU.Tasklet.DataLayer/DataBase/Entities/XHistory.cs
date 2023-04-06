using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;
using SBEU.Tasklet.Models.Enums;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XHistory : IEntity
    {
        public XHistory()
        {
            //Contents ??= new HashSet<XContent>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public virtual XTask Task { get; set; }
        public DateTime UpdateDate { get; set; }
        [Required,NotNull]
        public virtual XIdentityUser Updater { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public virtual TaskProgress? Status { get; set; }
        public virtual XIdentityUser? Executor { get; set; }
        public virtual ICollection<XContent> Contents { get; set; }
    }
}
