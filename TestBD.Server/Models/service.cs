using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestBD.Server.Models;

[Table("service")]
public partial class Service
{
    [Key]
    [Column("service_code")]
    [StringLength(25)]
    public string ServiceCode { get; set; } = null!;

    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("base_price", TypeName = "money")]
    public decimal BasePrice { get; set; }

    [Column("service_name")]
    [StringLength(100)]
    public string ServiceName { get; set; } = null!;

    [InverseProperty("ServiceCodeNavigation")]
    public virtual ICollection<Providedservice> Providedservices { get; set; } = new List<Providedservice>();
}
