using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("providedservice")]
[Index("AppointmentId", Name = "idx_providedservice_appointment_id")]
[Index("ContractorId", Name = "idx_providedservice_contractor_id")]
[Index("ProvidedServiceId", Name = "uniq_service_uuid", IsUnique = true)]
public partial class Providedservice
{
    [Column("service_code")]
    [StringLength(25)]
    public string ServiceCode { get; set; } = null!;

    [Column("fact_price", TypeName = "money")]
    public decimal FactPrice { get; set; }

    [Column("provided_date")]
    public DateTime ProvidedDate { get; set; }

    [Key]
    [Column("provided_service_id")]
    public Guid? ProvidedServiceId { get; set; }

    [Column("appointment_id")]
    public Guid AppointmentId { get; set; }

    [Column("contractor_id")]
    public Guid ContractorId { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Providedservices")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("ContractorId")]
    [InverseProperty("Providedservices")]
    public virtual Employee Contractor { get; set; } = null!;

    [InverseProperty("ProvidedService")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("ServiceCode")]
    [InverseProperty("Providedservices")]
    public virtual Service ServiceCodeNavigation { get; set; } = null!;
}
