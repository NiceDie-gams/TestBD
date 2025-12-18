namespace TestBD.Server.Dtos
{
    public class CreateScheduleDto
    {
        public Guid doctorId { get; set; }
        public int cabinetNumber { get; set; }
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
    }
}
