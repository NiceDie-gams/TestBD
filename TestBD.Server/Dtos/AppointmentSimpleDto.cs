namespace TestBD.Server.Dtos
{
    public class AppointmentSimpleDto
    {
        public Guid? AppointmentId { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public PatientSimpleDto Patient { get; set; } = null!;
    }
}
