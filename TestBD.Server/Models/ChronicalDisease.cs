using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("ChronicalDisease")]
public partial class ChronicalDisease
{
    [Key]
    [Column("disease_id")]
    public int DiseaseId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("idc_code")]
    [StringLength(25)]
    public string IdcCode { get; set; } = null!;

    [InverseProperty("Disease")]
    public virtual ICollection<PatientDisease> PatientDiseases { get; set; } = new List<PatientDisease>();
}
