using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("employeeiconimage")]
public partial class Employeeiconimage
{
    [Key]
    [Column("image_id")]
    public int ImageId { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("employee_id")]
    public Guid EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Employeeiconimages")]
    public virtual Employee Employee { get; set; } = null!;
}
