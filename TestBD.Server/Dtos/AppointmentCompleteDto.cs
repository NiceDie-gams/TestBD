namespace TestBD.Server.Dtos
{
    public class AppointmentCompleteDto
    {
        public Guid AppointmentId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateOnly? AppointmentDate { get; set; }

        public PatientSimpleDto Patient { get; set; } = null!;

        public ScheduleNoteSimpleDto? ScheduleNote { get; set; }
    }
}
