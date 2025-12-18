using TestBD.Server.Models.Enums;

namespace TestBD.Server.Dtos
{
    public class VisitHistoryDto
    {
        public DateTime VisitDate { get; set; }

        public string? Prediagnose { get; set; }

        public string? Diagnose { get; set; }

        public Guid PatientId { get; set; }

        public string EmployeeFio { get; set; } = null!;

        public string VisitType { get; set; }
    }
}
