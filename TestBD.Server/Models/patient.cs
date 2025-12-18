using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("patient")]
[Index("ContactPhone", Name = "patient_contact_phone_key", IsUnique = true)]
[Index("OmsPolisNumber", Name = "patient_oms_polis_number_key", IsUnique = true)]
[Index("PatientId", Name = "uniq_patient_uuid", IsUnique = true)]
public partial class Patient
{
    [Column("patient_birthdate")]
    public DateOnly PatientBirthdate { get; set; }

    [Column("gender")]
    [StringLength(1)]
    public string Gender { get; set; } = null!;

    [Column("contact_phone")]
    [StringLength(13)]
    public string ContactPhone { get; set; } = null!;

    [Column("oms_polis_number")]
    public long? OmsPolisNumber { get; set; }

    [Column("registration_date")]
    public DateOnly RegistrationDate { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("patient_fio")]
    [StringLength(100)]
    public string PatientFio { get; set; } = null!;

    [Key]
    [Column("patient_id")]
    public Guid? PatientId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<AllergiesPatient> AllergiesPatients { get; set; } = new List<AllergiesPatient>();

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Patient")]
    public virtual ICollection<PatientDisease> PatientDiseases { get; set; } = new List<PatientDisease>();

    [ForeignKey("UserId")]
    [InverseProperty("Patients")]
    public virtual User? User { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}
