using TestBD.Server.Dtos;
using TestBD.Server.Models;
using TestBD.Server.Repository;

namespace TestBD.Server.Services
{
    public class PatientService(RepositoryPatient repository)
    {
        public async Task<ICollection<PatientDto>> GetPatientsAsync()
        {
            return await repository.GetAllPatients();
        }

        public async Task AddPatientAsync(PatientDto patient)
        {
            await repository.AddPatientAsync(patient);
        }

        public async Task<PatientDto> GetById(Guid id)
        {
            return await repository.GetById(id);
        }

        public async Task UpdateAsync(PatientDto request)
        {
            await repository.UpdatePatientInfoAsync(request);
        }

        public async Task DeleteAsync(Guid id)
        {
            await repository.DeleteAsync(id);
        }
    }
}
