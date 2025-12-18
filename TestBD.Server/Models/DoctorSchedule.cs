using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("doctor_schedule")]
[Index("PointDate", Name = "idx_doctor_schedule_date")]
[Index("DoctorId", Name = "idx_doctor_schedule_doctor_id")]
[Index("ScheduleNoteId", Name = "uniq_schedule_uuid", IsUnique = true)]
public partial class DoctorSchedule
{
    [Column("point_date")]
    public DateOnly PointDate { get; set; }

    [Column("start_time", TypeName = "time without time zone")]
    public TimeOnly StartTime { get; set; }

    [Column("cabinet_number")]
    public int CabinetNumber { get; set; }

    [Column("end_time", TypeName = "time without time zone")]
    public TimeOnly EndTime { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; }

    [Key]
    [Column("schedule_note_id")]
    public Guid ScheduleNoteId { get; set; }

    [Column("doctor_id")]
    public Guid DoctorId { get; set; }

    [InverseProperty("Schedule")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("CabinetNumber")]
    [InverseProperty("DoctorSchedules")]
    public virtual Cabinet CabinetNumberNavigation { get; set; } = null!;

    [ForeignKey("DoctorId")]
    [InverseProperty("DoctorSchedules")]
    public virtual Employee Doctor { get; set; } = null!;
}
