using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestToday.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a visittype entity with essential details
    /// </summary>
    public class VisitType
    {
        /// <summary>
        /// Default of the VisitType 
        /// </summary>
        public bool? Default { get; set; }
        /// <summary>
        /// TenantId of the VisitType 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Primary key for the VisitType 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Required field Name of the VisitType 
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}