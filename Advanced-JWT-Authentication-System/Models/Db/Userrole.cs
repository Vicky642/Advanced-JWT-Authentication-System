using System;
using System.Collections.Generic;

#nullable disable

namespace Advanced_JWT_Authentication_System.Models.Db
{
    public partial class Userrole
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
