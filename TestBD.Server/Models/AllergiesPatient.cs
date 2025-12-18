using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[PrimaryKey("PatientId", "AllergyId")]
[Table("allergies_patients")]
public partial class AllergiesPatient
{
    [Key]
    [Column("allergy_id")]
    [StringLength(25)]
    public string AllergyId { get; set; } = null!;

    [Column("grade")]
    [StringLength(15)]
    public string Grade { get; set; } = null!;

    [Column("diagnostic_date")]
    public DateOnly DiagnosticDate { get; set; }

    [Column("notes")]
    public string Notes { get; set; } = null!;

    [Key]
    [Column("patient_id")]
    public Guid PatientId { get; set; }

    [ForeignKey("AllergyId")]
    [InverseProperty("AllergiesPatients")]
    public virtual Allergy Allergy { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("AllergiesPatients")]
    public virtual Patient Patient { get; set; } = null!;
}
