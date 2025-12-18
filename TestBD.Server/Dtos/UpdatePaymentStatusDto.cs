namespace TestBD.Server.Dtos
{
    public class UpdatePaymentStatusDto
    {
        public Guid PaymentId { get; set; }
        public string Status { get; set; }
    }
}
