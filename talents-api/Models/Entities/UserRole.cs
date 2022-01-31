using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentsApi.Models.Entities
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime DateCreated { get; set; }
        public string Permission { get; set; }
        public bool IsAdmin { get; set; }
    }
}
