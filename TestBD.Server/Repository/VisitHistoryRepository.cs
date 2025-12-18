using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class VisitHistoryRepository
    {
        private DbContext DbContext { get; }
        private DbSet<VisitHistory> histories {  get; }
        private readonly IMapper _mapper;

        public VisitHistoryRepository(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            _mapper = mapper;
            histories = DbContext.Set<VisitHistory>();
        }

        public async Task<ICollection<VisitHistoryDto>> GetByPatientId(Guid patientId)
        {
            var history = await histories.Where(x => x.PatientId == patientId).ProjectTo<VisitHistoryDto>(_mapper.ConfigurationProvider).ToListAsync();
            return history;
        }

        public async Task AddAsync(VisitHistoryRequestDto request)
        {
            var historyNote = _mapper.Map<VisitHistory>(request);
            await histories.AddAsync(historyNote);
            await DbContext.SaveChangesAsync();
        }
    }
}
