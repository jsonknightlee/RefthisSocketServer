using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBNParser.Classes
{
    public class UserItem
    {
        public int UserID { get; set; }
        public string DeviceID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
