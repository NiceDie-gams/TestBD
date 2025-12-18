using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Models;
using TestBD.Server.Models.Enums;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TestBD.Server.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AllergiesPatient> AllergiesPatients { get; set; }

    public virtual DbSet<Allergy> Allergies { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Cabinet> Cabinets { get; set; }

    public virtual DbSet<ChronicalDisease> ChronicalDiseases { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Employeeiconimage> Employeeiconimages { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientDisease> PatientDiseases { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Providedservice> Providedservices { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VisitHistory> VisitHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=MedicalClinicDatabase;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("C")
            .HasPostgresEnum("AppointmentType", new[] { "Платный", "ОМС" })
            .HasPostgresEnum("PaymentConfiguration", new[] { "ОМС", "Платный", "Комбинированная" })
            .HasPostgresEnum("PaymentStatus", new[] { "Paied", "Waiting", "По ОМС", "Canceled" })
            .HasPostgresEnum("PaymentType", new[] { "Безналичные", "Наличные", "Онлайн" })
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AllergiesPatient>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.AllergyId }).HasName("allergies_patients_pkey");

            entity.HasOne(d => d.Allergy).WithMany(p => p.AllergiesPatients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patients_allergies_allergies_id");

            entity.HasOne(d => d.Patient).WithMany(p => p.AllergiesPatients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_allergies_patients_patient_uuid");
        });

        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyCode).HasName("Allergies_pkey");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("appointment_pkey");

            entity.Property(e => e.AppointmentId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValueSql("'booked'::character varying");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appointment_patient_uuid");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appointment_schedule_uuid");
        });

        modelBuilder.Entity<Cabinet>(entity =>
        {
            entity.HasKey(e => e.CabinetNumber).HasName("cabinet_pkey");

            entity.Property(e => e.CabinetNumber).ValueGeneratedNever();
        });

        modelBuilder.Entity<ChronicalDisease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("ChronicalDisease_pkey");

            entity.Property(e => e.DiseaseId).HasDefaultValueSql("nextval('\"ChronicalDesease_desease_id_seq\"'::regclass)");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleNoteId).HasName("doctor_schedule_pkey");

            entity.Property(e => e.ScheduleNoteId).ValueGeneratedNever();
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(d => d.CabinetNumberNavigation).WithMany(p => p.DoctorSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cabinet_schedule");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedule_employee_uuid");

        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employee_pkey");

            entity.Property(e => e.EmployeeId).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(d => d.User).WithMany(p => p.Employees).HasConstraintName("fk_user_id_employee");
        });

        modelBuilder.Entity<Employeeiconimage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("employeeiconimage_pkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Employeeiconimages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_iconImage");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("patient_pkey");

            entity.Property(e => e.PatientId).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.Patients).HasConstraintName("fk_user_id_patient");
        });

        modelBuilder.Entity<PatientDisease>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.DiseaseId }).HasName("patient_disease_pkey");

            entity.Property(e => e.Activity).HasDefaultValue(true);

            entity.HasOne(d => d.Disease).WithMany(p => p.PatientDiseases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient_disease_disease_id");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientDiseases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient_disease_patient_uuid");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payment_pkey");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();

            entity.HasOne(d => d.ProvidedService).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_service_uuid");
        });

        modelBuilder.Entity<Providedservice>(entity =>
        {
            entity.HasKey(e => e.ProvidedServiceId).HasName("providedservice_pkey");

            entity.Property(e => e.ProvidedServiceId).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Providedservices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_service_appointment_uuid");

            entity.HasOne(d => d.Contractor).WithMany(p => p.Providedservices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_service_employee_uuid");

            entity.HasOne(d => d.ServiceCodeNavigation).WithMany(p => p.Providedservices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_service_provided");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceCode).HasName("service_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.Property(e => e.UserId).HasDefaultValueSql("uuid_generate_v4()");
        });

        modelBuilder.Entity<VisitHistory>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("visitHistory_pkey");

            entity.Property(e => e.NoteId).HasDefaultValueSql("uuid_generate_v4()");

            entity.HasOne(d => d.Appointment).WithMany(p => p.VisitHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visithistory_appointment_uuid");

            entity.HasOne(d => d.Patient).WithMany(p => p.VisitHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visithistory_patient_uuid");
            entity.Property(d => d.visitType).HasColumnName("visit_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
