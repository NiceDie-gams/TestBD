using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Models.Enums;

namespace TestBD.Server.Models;

[Table("visitHistory")]
[Index("AppointmentId", Name = "idx_visithistory_appointment_id")]
[Index("PatientId", Name = "idx_visithistory_patient_id")]
[Index("NoteId", Name = "uniq_note_uuid", IsUnique = true)]
public partial class VisitHistory
{
    [Column("visit_date")]
    public DateTime VisitDate { get; set; }

    [Column("prediagnose")]
    public string? Prediagnose { get; set; }

    [Column("diagnose")]
    public string? Diagnose { get; set; }

    [Column("visit_type")]
    [StringLength(30)]
    public string visitType { get; set; } = null!;

    [Key]
    [Column("note_id")]
    public Guid? NoteId { get; set; }

    [Column("patient_id")]
    public Guid PatientId { get; set; }

    [Column("appointment_id")]
    public Guid AppointmentId { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("VisitHistories")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("VisitHistories")]
    public virtual Patient Patient { get; set; } = null!;
}
