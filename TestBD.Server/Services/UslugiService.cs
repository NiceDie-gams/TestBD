using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class UslugiService(ServiceRepository repository)
    {
        public async Task<ICollection<ServiceDto>> GetAllServices()
        {
            return await repository.GetAllServices();
        }

    }
}
