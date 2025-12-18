namespace TestBD.Server.Dtos
{
    public class PatientSimpleDto
    {
        public Guid? PatientId { get; set; }
        public string PatientFio { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
    }
}
