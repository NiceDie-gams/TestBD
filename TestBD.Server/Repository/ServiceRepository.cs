using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class ServiceRepository
    {
        private DbContext dbContext { get; }
        private DbSet<Service> services { get; }
        private readonly IMapper _mapper;

        public ServiceRepository(AppDbContext _dbContext, IMapper mapper)
        {
            dbContext = _dbContext;
            _mapper = mapper;
            services = dbContext.Set<Service>();
        }

        public async Task<ICollection<ServiceDto>> GetAllServices()
        {
            var service = await services.ProjectTo<ServiceDto>(_mapper.ConfigurationProvider).ToListAsync();
            return service;
        }
    }
}
