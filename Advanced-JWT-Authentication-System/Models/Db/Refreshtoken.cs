using System;
using System.Collections.Generic;

#nullable disable

namespace Advanced_JWT_Authentication_System.Models.Db
{
    public partial class Refreshtoken
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public ulong? IsRevoked { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
