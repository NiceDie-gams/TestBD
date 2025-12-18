using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("users")]
[Index("Login", Name = "unique_login", IsUnique = true)]
public partial class User
{
    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("role")]
    [StringLength(20)]
    public string? Role { get; set; }

    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("login")]
    [MaxLength(50)]
    public string Login { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("User")]
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
