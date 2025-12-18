using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class PaymentService(PaymentRepository repository)
    {
        public async Task<ICollection<PaymentDto>> GetAllAsync()
        {
            return await repository.GetAllPayments();
        }

        public async Task UpdateStatusAsync(UpdatePaymentStatusDto dto)
        {
            await repository.UpdateStatusAsync(dto);
        }
    }
}
