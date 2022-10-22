using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XMessage : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public virtual XIdentityUser From { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual XTable? Table { get; set; }
        public virtual XTask? Task { get; set; }
    }
}
