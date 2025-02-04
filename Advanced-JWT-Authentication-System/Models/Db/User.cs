using System;
using System.Collections.Generic;

#nullable disable

namespace Advanced_JWT_Authentication_System.Models.Db
{
    public partial class User
    {
        public long Id { get; set; }
        public string FullName { get; set; }        
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public ulong? IsEmailVerified { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? VerificationExpiry { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public ulong? IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
