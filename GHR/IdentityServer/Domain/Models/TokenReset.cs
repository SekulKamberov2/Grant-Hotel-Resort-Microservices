namespace IdentityServer.Domain.Models
{
    using System;
    using System.ComponentModel.DataAnnotations; 
    public class TokenReset
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string Token { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public bool IsUsed { get; set; }

        public TokenReset(int userId, string token, DateTime expiryDate, DateTime createdDate, bool isUsed)
        {
            UserId = userId;
            Token = token;
            ExpiryDate = expiryDate;
            CreatedDate = createdDate;
            IsUsed = isUsed;
        }
    }

}
