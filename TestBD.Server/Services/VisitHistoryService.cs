using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class VisitHistoryService(VisitHistoryRepository repository)
    {
        public async Task<ICollection<VisitHistoryDto>> GetByPatientId(Guid patientId)
        {
            return await repository.GetByPatientId(patientId);
        }

        public async Task AddAsync(VisitHistoryRequestDto request)
        {
            await repository.AddAsync(request);
        }
    }
}
