using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("payment")]
[Index("PaymentDate", Name = "idx_payment_date")]
[Index("ProvidedServiceId", Name = "idx_payment_service_id")]
[Index("PaymentId", Name = "uniq_payment_uuid", IsUnique = true)]
public partial class Payment
{
    [Column("summary_price", TypeName = "money")]
    public decimal SummaryPrice { get; set; }

    [Column("payment_date")]
    public DateTime PaymentDate { get; set; }

    [Column("status")]

    public string Status { get; set; }

    [Key]
    [Column("payment_id")]
    public Guid PaymentId { get; set; }

    [Column("provided_service_id")]
    public Guid ProvidedServiceId { get; set; }

    [ForeignKey("ProvidedServiceId")]
    [InverseProperty("Payments")]
    public virtual Providedservice ProvidedService { get; set; } = null!;
}
