using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class ProvidedServiceRepository
    {
        private DbContext DbContext { get; }
        private DbSet<Providedservice> services { get; }
        private readonly IMapper _mapper;

        public ProvidedServiceRepository(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            _mapper = mapper;
            services = dbContext.Set<Providedservice>();
        }

        public async Task<ICollection<ProvidedServiceDto>> GetByAppointmentId(Guid appointmentId)
        {
            var service = await services
                .Where(x => x.AppointmentId == appointmentId)
                .ProjectTo<ProvidedServiceDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return service;
        }

        public async Task AddService(ProvidedServiceDto request)
        {
            var service = _mapper.Map<Providedservice>(request);
            await services.AddAsync(service);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateService(ProvidedServiceDto request)
        {
            var service = _mapper.Map<Providedservice>(request);
            services.Update(service);
            await DbContext.SaveChangesAsync();
        }
    }
}
