namespace TestBD.Server.Dtos
{
    public class ScheduleNoteSimpleDto
    {
        public DateOnly PointDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? CabinetNumber { get; set; }
    }
}
