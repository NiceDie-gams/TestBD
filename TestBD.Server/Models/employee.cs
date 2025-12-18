using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("employee")]
[Index("Email", Name = "emailUNIQUE", IsUnique = true)]
[Index("Phone", Name = "phoneNumberUNIQUE", IsUnique = true)]
[Index("EmployeeId", Name = "uniq_employee_uuid", IsUnique = true)]
public partial class Employee
{
    [Column("employee_fio")]
    public string EmployeeFio { get; set; } = null!;

    [Column("post")]
    [StringLength(100)]
    public string Post { get; set; } = null!;

    [Column("specialization")]
    public string Specialization { get; set; } = null!;

    [Column("experience")]
    public int Experience { get; set; }

    [Column("phone")]
    [StringLength(15)]
    public string? Phone { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Key]
    [Column("employee_id")]
    public Guid? EmployeeId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    [InverseProperty("Employee")]
    public virtual ICollection<Employeeiconimage> Employeeiconimages { get; set; } = new List<Employeeiconimage>();

    [InverseProperty("Contractor")]
    public virtual ICollection<Providedservice> Providedservices { get; set; } = new List<Providedservice>();

    [ForeignKey("UserId")]
    [InverseProperty("Employees")]
    public virtual User? User { get; set; }
}
