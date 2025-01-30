using System;
using System.Collections.Generic;

#nullable disable

namespace Advanced_JWT_Authentication_System.Models.Db
{
    public partial class Auditlog
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Action { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
