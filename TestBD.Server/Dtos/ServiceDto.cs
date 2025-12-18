namespace TestBD.Server.Dtos
{
    public class ServiceDto
    {
        public string ServiceCode { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public string ServiceName { get; set; } = null!;
    }
}
