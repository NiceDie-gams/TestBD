namespace TestBD.Server.Dtos
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = "Waiting";
        public string PatientFio {  get; set; }
        public decimal summaryPrice {  get; set; }
    }
}
