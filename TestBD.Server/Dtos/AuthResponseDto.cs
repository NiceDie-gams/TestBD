namespace TestBD.Server.Dtos
{
    //public class AuthResponseDto
    //{
    //    public string Token { get; set; } = null!;
    //    public string Login { get; set; } = null!;
    //    public string Role { get; set; } = null!;
    //    public DateTime Expires { get; set; }
    //}

    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime Expires { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
