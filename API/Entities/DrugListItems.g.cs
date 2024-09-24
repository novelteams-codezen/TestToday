using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestToday.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a druglistitems entity with essential details
    /// </summary>
    public class DrugListItems
    {
        /// <summary>
        /// Primary key for the DrugListItems 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// TenantId of the DrugListItems 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Required field Name of the DrugListItems 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Foreign key referencing the Medication to which the DrugListItems belongs 
        /// </summary>
        public Guid? MedicationId { get; set; }

        /// <summary>
        /// Navigation property representing the associated Medication
        /// </summary>
        [ForeignKey("MedicationId")]
        public Medication? MedicationId_Medication { get; set; }
    }
}