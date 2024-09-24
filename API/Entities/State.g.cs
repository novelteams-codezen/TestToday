using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestToday.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a state entity with essential details
    /// </summary>
    public class State
    {
        /// <summary>
        /// Primary key for the State 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// TenantId of the State 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Required field Name of the State 
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}