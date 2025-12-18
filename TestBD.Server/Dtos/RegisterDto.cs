namespace TestBD.Server.Dtos
{
    public class RegisterDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "User";

        // Данные пациента
        public string? PatientFio { get; set; }
        public DateOnly? PatientBirthdate { get; set; }
        public string? Gender { get; set; }
        public string? ContactPhone { get; set; }

        // Данные врача
        public string? EmployeeFio { get; set; }
        public string? Post { get; set; }
        public string? Specialization { get; set; }
        public int? Experience { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
