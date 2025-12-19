using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class PaymentRepository
    {
        private DbContext DbContext { get; set; } 
        private DbSet<Payment> payments { get; set; }
        
        private readonly IMapper _mapper;

        public PaymentRepository(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            _mapper = mapper;
            payments = DbContext.Set<Payment>();
        }

        public async Task<ICollection<PaymentDto>> GetAllPayments()
        {
            var responce = await payments.ProjectTo<PaymentDto>(_mapper.ConfigurationProvider).ToListAsync();
            return responce;
        }

        public async Task UpdateStatusAsync(UpdatePaymentStatusDto dto)
        {
            string[] allowedStatuses = { "Paied", "Waiting", "Cancelled" };
            var payment = await payments.FirstOrDefaultAsync(x => x.PaymentId == dto.PaymentId);
            if (!allowedStatuses.Contains(dto.Status)) {
                throw new Exception("No such status for payment");
            }
            else {
                payment.Status = dto.Status;
            }

            payments.Update(payment);
            await DbContext.SaveChangesAsync();
        }
    }
}
