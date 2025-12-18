namespace TestBD.Server.Dtos
{
    public class PatientDto
    {
        public DateOnly PatientBirthdate { get; set; }

        public string Gender { get; set; } = null!;

        public string ContactPhone { get; set; } = null!;

        public long? OmsPolisNumber { get; set; }

        public DateOnly RegistrationDate { get; set; }

        public bool? IsActive { get; set; }

        public string PatientFio { get; set; } = null!;

        public Guid? PatientId { get; set; }
    }
}
