using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .Include(u => u.Patients) // Включаем пациента
            .Include(u => u.Employees) // Включаем врача
            .FirstOrDefaultAsync(u => u.Login == loginDto.Login);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Неверный логин или пароль");

        return await GenerateJwtTokenAsync(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await UserExistsAsync(registerDto.Login))
            throw new InvalidOperationException("Пользователь с таким логином уже существует");

        var allowedRoles = new[] { "admin", "user", "employee" };
        var role = string.IsNullOrEmpty(registerDto.Role) ? "user" : registerDto.Role.ToLower();

        if (!allowedRoles.Contains(role))
            throw new InvalidOperationException("Недопустимая роль");

        // 1. Создаём User
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Login = registerDto.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 2. В зависимости от роли создаём пациента или врача
        if (role == "user")
        {
            var patient = new Patient
            {
                PatientId = Guid.NewGuid(),
                PatientFio = registerDto.PatientFio ?? "Не указано",
                PatientBirthdate = registerDto.PatientBirthdate ?? DateOnly.FromDateTime(DateTime.Now),
                Gender = registerDto.Gender ?? "M",
                ContactPhone = registerDto.ContactPhone ?? "0000000000",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true,
                UserId = user.UserId
            };

            _context.Patients.Add(patient);
        }
        else if (role == "employee")
        {
            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid(),
                EmployeeFio = registerDto.EmployeeFio ?? "Не указано",
                Post = registerDto.Post ?? "Не указано",
                Specialization = registerDto.Specialization ?? "Не указано",
                Experience = registerDto.Experience ?? 0,
                Phone = registerDto.Phone,
                Email = registerDto.Email,
                UserId = user.UserId
            };

            _context.Employees.Add(employee);
        }

        await _context.SaveChangesAsync();

        // 🔴 ПЕРЕЗАГРУЖАЕМ пользователя с включенными связанными данными
        var userWithRelations = await _context.Users
            .Include(u => u.Patients)
            .Include(u => u.Employees)
            .FirstOrDefaultAsync(u => u.UserId == user.UserId);

        if (userWithRelations == null)
        {
            throw new InvalidOperationException("Ошибка при создании пользователя");
        }

        return await GenerateJwtTokenAsync(userWithRelations);
    }

    public async Task<bool> UserExistsAsync(string login)
    {
        return await _context.Users.AnyAsync(u => u.Login == login);
    }

    // AuthService.cs - исправленный метод GenerateJwtTokenAsync

    private async Task<AuthResponseDto> GenerateJwtTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key не настроен")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Загружаем свежие данные из базы
        var userWithRelations = await _context.Users
            .Include(u => u.Patients)
            .Include(u => u.Employees)
            .FirstOrDefaultAsync(u => u.UserId == user.UserId);

        if (userWithRelations == null)
        {
            throw new InvalidOperationException("Пользователь не найден");
        }

        var patient = userWithRelations.Patients?.FirstOrDefault();
        var employee = userWithRelations.Employees?.FirstOrDefault();

        // 🔴 ИСПРАВЛЕНО: Используем стандартные ClaimTypes
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userWithRelations.UserId.ToString()),
        new Claim(ClaimTypes.Name, userWithRelations.Login),
        new Claim(ClaimTypes.Role, userWithRelations.Role ?? "user"),
        // Добавляем кастомные claims
        new Claim("userId", userWithRelations.UserId.ToString()),
        new Claim("login", userWithRelations.Login)
    };

        // Добавляем patientId если есть
        if (patient != null)
        {
            claims.Add(new Claim("patientId", patient.PatientId.ToString()));
            claims.Add(new Claim("patientFio", patient.PatientFio));
        }

        // Добавляем employeeId если есть
        if (employee != null)
        {
            claims.Add(new Claim("employeeId", employee.EmployeeId.ToString()));
            claims.Add(new Claim("employeeFio", employee.EmployeeFio));
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Login = userWithRelations.Login,
            Role = userWithRelations.Role,
            Expires = token.ValidTo,
            UserId = userWithRelations.UserId,
            PatientId = patient?.PatientId,
            EmployeeId = employee?.EmployeeId
        };
    }
    //private async Task<AuthResponseDto> GenerateJwtTokenAsync(User user)
    //{
    //    var key = new SymmetricSecurityKey(
    //        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
    //            ?? throw new InvalidOperationException("JWT Key не настроен")));

    //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    // 🔴 ВАЖНО: Загружаем свежие данные из базы с пациентами/врачами
    //    var userWithRelations = await _context.Users
    //        .Include(u => u.Patients)
    //        .Include(u => u.Employees)
    //        .FirstOrDefaultAsync(u => u.UserId == user.UserId);

    //    if (userWithRelations == null)
    //    {
    //        throw new InvalidOperationException("Пользователь не найден");
    //    }

    //    var patient = userWithRelations.Patients?.FirstOrDefault();
    //    var employee = userWithRelations.Employees?.FirstOrDefault();

    //    var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.NameIdentifier, userWithRelations.UserId.ToString()),
    //    new Claim(ClaimTypes.Name, userWithRelations.Login),
    //    new Claim(ClaimTypes.Role, userWithRelations.Role ?? "user"),
    //    new Claim("userId", userWithRelations.UserId.ToString()),
    //    new Claim("login", userWithRelations.Login),
    //    new Claim("role", userWithRelations.Role ?? "user")
    //};

    //    // 🔴 ДОБАВЛЯЕМ patientId если есть
    //    if (patient != null)
    //    {
    //        claims.Add(new Claim("patientId", patient.PatientId.ToString()));
    //        claims.Add(new Claim("patientFio", patient.PatientFio));
    //    }

    //    // 🔴 ДОБАВЛЯЕМ employeeId если есть
    //    if (employee != null)
    //    {
    //        claims.Add(new Claim("employeeId", employee.EmployeeId.ToString()));
    //        claims.Add(new Claim("employeeFio", employee.EmployeeFio));
    //    }

    //    var token = new JwtSecurityToken(
    //        //issuer: _configuration["Jwt:Issuer"],
    //        //audience: _configuration["Jwt:Audience"],
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddHours(12),
    //        signingCredentials: credentials
    //    );

    //    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    //    return new AuthResponseDto
    //    {
    //        Token = tokenString,
    //        Login = userWithRelations.Login,
    //        Role = userWithRelations.Role,
    //        Expires = token.ValidTo,
    //        UserId = userWithRelations.UserId,
    //        PatientId = patient?.PatientId,
    //        EmployeeId = employee?.EmployeeId
    //    };
}

