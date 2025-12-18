using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("cabinet")]
public partial class Cabinet
{
    [Key]
    [Column("cabinet_number")]
    public int CabinetNumber { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [InverseProperty("CabinetNumberNavigation")]
    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
}
