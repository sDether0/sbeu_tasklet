using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XIdentityUserConfirm : IEntity
    {
        public string Id { get; set; }
        public virtual XIdentityUser User { get; set; }
        public string HashCode { get; set; }
        public string MailCode { get; set; } = RandomString(5);
        
        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
