using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class EmployeeService(EmployeeRepository repository)
    {
        public async Task<ICollection<EmployeeDto>> GetAllDoctors()
        {
            return await repository.GetAllDoctors();
        }

        public async Task<ICollection<EmployeeDto>> GetEmployeeByPost(string post)
        {
            return await repository.GetDoctorByPost(post);
        }

        public async Task UpdateAsync(EmployeeDto request)
        {
            await repository.UpdateAsync(request);
        }

        public async Task AddAsync(EmployeeDto request)
        {
            await repository.AddAsync(request);
        }

        public async Task DeleteAsync(Guid id)
        {
            await repository.DeleteAsync(id);
        }
    }
}
