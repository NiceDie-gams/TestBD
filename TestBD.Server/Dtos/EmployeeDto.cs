namespace TestBD.Server.Dtos
{
    public class EmployeeDto
    {
        public string EmployeeFio { get; set; } = null!;

        public string Post { get; set; } = null!;

        public string Specialization { get; set; } = null!;

        public int Experience { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public Guid? EmployeeId { get; set; }
    }
}
