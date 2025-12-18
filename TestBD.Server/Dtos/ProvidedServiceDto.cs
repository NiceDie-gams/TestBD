namespace TestBD.Server.Dtos
{
    public class ProvidedServiceDto
    {
        public string ServiceCode { get; set; } = null!;

        public decimal FactPrice { get; set; }

        public DateTime ProvidedDate { get; set; }

        public Guid? ProvidedServiceId { get; set; }

        public Guid AppointmentId { get; set; }

        public Guid ContractorId { get; set; }
    }
}
