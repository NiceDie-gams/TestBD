using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

namespace TestBD.Server.Repository
{
    public class RepositoryPatient
    {
        private DbContext dbContext { get; }
        private DbSet<Patient> patients  { get; }
        private readonly IMapper _mapper;

        public RepositoryPatient(AppDbContext _dbContext, IMapper mapper)
        {
            dbContext = _dbContext;
            _mapper = mapper;
            patients = dbContext.Set<Patient>();
        }

        public async Task<ICollection<PatientDto>> GetAllPatients()
        {
            var patient = await patients.Where(x => x.IsActive == true).ProjectTo<PatientDto>(_mapper.ConfigurationProvider).ToListAsync();
            return patient;
        }

        public async Task AddPatientAsync( PatientDto patient)
        {
            var newPatient = _mapper.Map<Patient>(patient);
            await patients.AddAsync(newPatient);
            await dbContext.SaveChangesAsync();
        }

        public async Task<PatientDto> GetById(Guid id)
        {
            var patient = await patients
           .Where(p => p.PatientId == id) // Добавляем фильтрацию по ID
           .ProjectTo<PatientDto>(_mapper.ConfigurationProvider)
           .FirstOrDefaultAsync();

            if (patient == null)
            {
                throw new KeyNotFoundException($"Пациент с ID {id} не найден");
            }
            return patient;
        }

        public async Task UpdatePatientInfoAsync(PatientDto request)
        {
            //var service = _mapper.Map<Patient>(request);
            //patients.Update(service);
            //await dbContext.SaveChangesAsync();
            // Находим существующего пациента
            var existingPatient = await patients
                .FirstOrDefaultAsync(p => p.PatientId == request.PatientId);

            if (existingPatient == null)
            {
                throw new KeyNotFoundException($"Пациент с ID {request.PatientId} не найден");
            }

            // Обновляем только разрешенные для изменения поля
            existingPatient.PatientFio = request.PatientFio;
            existingPatient.PatientBirthdate = request.PatientBirthdate;
            existingPatient.Gender = request.Gender;
            existingPatient.ContactPhone = request.ContactPhone;
            existingPatient.OmsPolisNumber = request.OmsPolisNumber;

            // Эти поля обычно не меняются пациентом, но оставим возможность
            existingPatient.IsActive = request.IsActive ?? existingPatient.IsActive;

            // Не обновляем:
            // - RegistrationDate (устанавливается при создании)
            // - PatientId (первичный ключ)
            // - UserId (связь с пользователем)

            patients.Update(existingPatient);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var patient = await patients.AsNoTracking().Where(x => x.PatientId == id).FirstOrDefaultAsync();
            if (patient == null)
            {
                throw new Exception("No such patient exist");
            }
            patient.IsActive = false;
            patients.Update(patient);
            await dbContext.SaveChangesAsync();
        }
    }
}
