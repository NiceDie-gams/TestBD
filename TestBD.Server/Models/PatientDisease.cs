using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[PrimaryKey("PatientId", "DiseaseId")]
[Table("patient_disease")]
public partial class PatientDisease
{
    [Key]
    [Column("disease_id")]
    public int DiseaseId { get; set; }

    [Column("activity")]
    public bool Activity { get; set; }

    [Column("diagnose_date")]
    public DateOnly DiagnoseDate { get; set; }

    [Key]
    [Column("patient_id")]
    public Guid PatientId { get; set; }

    [ForeignKey("DiseaseId")]
    [InverseProperty("PatientDiseases")]
    public virtual ChronicalDisease Disease { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("PatientDiseases")]
    public virtual Patient Patient { get; set; } = null!;
}
