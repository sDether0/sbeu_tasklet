﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class Note : DeletableEntity
    {
        public string TaskId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}
