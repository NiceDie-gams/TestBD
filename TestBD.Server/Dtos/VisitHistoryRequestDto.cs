using TestBD.Server.Models.Enums;

namespace TestBD.Server.Dtos
{
    public class VisitHistoryRequestDto
    {
        public DateTime VisitDate { get; set; }

        public string? Prediagnose { get; set; }

        public Guid? PatientId { get; set; }

        public Guid? AppointmentId { get; set; }

        public string VisitType { get; set; }

        public string? Diagnose { get; set; }

    }
}
