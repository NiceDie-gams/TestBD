using System.ComponentModel.DataAnnotations.Schema;

namespace TestBD.Server.Dtos
{
    public class DoctorScheduleDto
    {
        public DateOnly PointDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public int CabinetNumber { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid ScheduleNoteId { get; set; }

    }
}
