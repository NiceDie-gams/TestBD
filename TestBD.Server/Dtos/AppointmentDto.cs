namespace TestBD.Server.Dtos
{
    public class AppointmentDto
    {
        public string Status { get; set; } = null!;
        
        public Guid? AppointmentId { get; set; }

        public DateOnly? AppointmentDate { get; set; }
        
        public TimeOnly? AppointmentTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public string EmployeeFio { get; set; } = null!;

        public string Cabinet { get; set; } = null!;
    }
}
