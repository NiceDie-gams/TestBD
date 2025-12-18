using System.Collections.ObjectModel;
using TestBD.Server.Dtos;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class ProvidedUslugaService(ProvidedServiceRepository repository)
    {
        public async Task AddServiceAsync(ProvidedServiceDto request)
        {
            await repository.AddService(request);
        }

        public async Task UpdateAsync(ProvidedServiceDto request)
        {
            await repository.UpdateService(request);
        }

        public async Task<ICollection<ProvidedServiceDto>> GetByAppointmentId(Guid appointmentId)
        {
             return await repository.GetByAppointmentId(appointmentId);
        }
    }
}
