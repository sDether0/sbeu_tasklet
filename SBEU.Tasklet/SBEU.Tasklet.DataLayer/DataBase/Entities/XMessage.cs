using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XMessage : DeletableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        [Required,NotNull]
        public virtual XIdentityUser From { get; set; }
        [Required, NotNull]
        public virtual Chat Chat { get; set; }
        public virtual XTable? Table { get; set; }
        public virtual XTask? Task { get; set; }
    }
}
