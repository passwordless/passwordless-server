using System;

namespace Service.Models
{
    public class TokenKey : PerTenant
    {
        public string Tenant { get; set; }
        public string KeyMaterial { get; set; }
        public int KeyId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}