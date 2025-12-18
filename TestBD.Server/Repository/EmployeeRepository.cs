using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class EmployeeRepository
    {
        private DbContext DbContext { get; }
        private DbSet<Employee> employies { get; }

        private readonly IMapper _mapper;

        public EmployeeRepository(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext; 
            _mapper = mapper;
            employies = DbContext.Set<Employee>();
        }  

        public async Task<ICollection<EmployeeDto>> GetAllDoctors()
        {
            var employee = await employies.ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider).ToListAsync();
            return employee;
        }

        public async Task<ICollection<EmployeeDto>> GetDoctorByPost(string Post)
        {
            var employee = await employies.ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider).AsNoTracking().Where(x => x.Post == Post).ToListAsync();
            return employee;
        }

        public async Task AddAsync(EmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            await employies.AddAsync(employee);
            await DbContext.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(EmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            employies.Update(employee);
            await DbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var employee = employies.AsNoTracking().FirstOrDefault(b => b.EmployeeId == id);
            if (employee != null) throw new Exception("No employee exists");
            employies.Remove(employee);
            await DbContext.SaveChangesAsync();
        }
    }

}
