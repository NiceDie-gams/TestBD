using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestBD.Server.Dtos
{
    public class AppointmentRequestDto
    {

        public string? Status { get; set; } = "booked";
        
        public DateTime CreatedAt { get; set; }
        
        public Guid PatientId { get; set; }

        public Guid ScheduleId { get; set; }
    }
}
