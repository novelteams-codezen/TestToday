using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestToday.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a bloodgroup entity with essential details
    /// </summary>
    public class BloodGroup
    {
        /// <summary>
        /// Primary key for the BloodGroup 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// TenantId of the BloodGroup 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Required field Name of the BloodGroup 
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}