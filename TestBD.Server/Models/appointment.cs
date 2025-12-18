using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("appointment")]
[Index("PatientId", Name = "idx_appointment_patient_id")]
[Index("ScheduleId", Name = "idx_appointment_schedule_id")]
[Index("AppointmentId", Name = "uniq_appointment_uuid", IsUnique = true)]
public partial class Appointment
{
    [Column("status")]
    [StringLength(30)]
    public string? Status { get; set; } = "booked";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Key]
    [Column("appointment_id")]
    public Guid? AppointmentId { get; set; }

    [Column("patient_id")]
    public Guid PatientId { get; set; }

    [Column("schedule_id")]
    public Guid ScheduleId { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient Patient { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<Providedservice> Providedservices { get; set; } = new List<Providedservice>();

    [ForeignKey("ScheduleId")]
    [InverseProperty("Appointments")]
    public virtual DoctorSchedule Schedule { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}
