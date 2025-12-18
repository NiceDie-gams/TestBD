using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("allergies")]
public partial class Allergy
{
    [Key]
    [Column("allergy_code")]
    [StringLength(25)]
    public string AllergyCode { get; set; } = null!;

    [Column("name")]
    [StringLength(75)]
    public string Name { get; set; } = null!;

    [Column("category")]
    [StringLength(50)]
    public string Category { get; set; } = null!;

    [InverseProperty("Allergy")]
    public virtual ICollection<AllergiesPatient> AllergiesPatients { get; set; } = new List<AllergiesPatient>();
}
