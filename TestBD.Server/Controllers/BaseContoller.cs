using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TestBD.Server.Controllers.Base
{
    /// <summary>
    /// Базовый контроллер с общими методами для работы с аутентификацией
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Проверяет, аутентифицирован ли пользователь
        /// </summary>
        protected bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Получает логин текущего пользователя
        /// </summary>
        protected string CurrentUsername
        {
            get
            {
                // Пробуем разные варианты имени
                var username = User?.FindFirst("login")?.Value
                            ?? User?.FindFirst(ClaimTypes.Name)?.Value
                            ?? User?.Identity?.Name;

                if (string.IsNullOrEmpty(username))
                    throw new UnauthorizedAccessException("User is not authenticated or login claim is missing");

                return username;
            }
        }

        /// <summary>
        /// Получает роль текущего пользователя
        /// </summary>
        protected string CurrentUserRole
        {
            get
            {
                // Пробуем разные варианты роли
                var role = User?.FindFirst("role")?.Value
                         ?? User?.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                    throw new UnauthorizedAccessException("Role claim is missing");

                return role;
            }
        }

        /// <summary>
        /// Получает ID пользователя из claims
        /// </summary>
        protected Guid CurrentUserId
        {
            get
            {
                var userIdClaim = User?.FindFirst("userId")?.Value
                                ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    throw new UnauthorizedAccessException("User ID claim is missing or invalid");

                return userId;
            }
        }

        /// <summary>
        /// Получает PatientId текущего пользователя (если есть)
        /// </summary>
        protected Guid? CurrentPatientId
        {
            get
            {
                var patientIdClaim = User?.FindFirst("patientId")?.Value;
                if (string.IsNullOrEmpty(patientIdClaim) || !Guid.TryParse(patientIdClaim, out var patientId))
                    return null;

                return patientId;
            }
        }

        /// <summary>
        /// Получает EmployeeId текущего пользователя (если есть)
        /// </summary>
        protected Guid? CurrentEmployeeId
        {
            get
            {
                var employeeIdClaim = User?.FindFirst("employeeId")?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !Guid.TryParse(employeeIdClaim, out var employeeId))
                    return null;

                return employeeId;
            }
        }

        /// <summary>
        /// Проверяет, является ли текущий пользователь администратором
        /// </summary>
        protected bool IsAdmin => CurrentUserRole == "admin";

        /// <summary>
        /// Проверяет, является ли текущий пользователь сотрудником
        /// </summary>
        protected bool IsEmployee => CurrentUserRole == "employee";

        /// <summary>
        /// Проверяет, является ли текущий пользователь обычным пользователем
        /// </summary>
        protected bool IsRegularUser => CurrentUserRole == "user";

        /// <summary>
        /// Получает все claims текущего пользователя
        /// </summary>
        protected IEnumerable<Claim> CurrentUserClaims => User?.Claims ?? Enumerable.Empty<Claim>();

        /// <summary>
        /// Получает значение конкретного claim
        /// </summary>
        protected string GetClaimValue(string claimType)
        {
            return User?.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Проверяет, имеет ли пользователь указанную роль
        /// </summary>
        protected bool HasRole(string role)
        {
            return CurrentUserRole == role;
        }

        /// <summary>
        /// Проверяет, имеет ли пользователь одну из указанных ролей
        /// </summary>
        protected bool HasAnyRole(params string[] roles)
        {
            return roles.Contains(CurrentUserRole);
        }

        /// <summary>
        /// Выбрасывает исключение, если пользователь не авторизован
        /// </summary>
        protected void EnsureAuthenticated()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated");
        }

        /// <summary>
        /// Выбрасывает исключение, если пользователь не имеет указанной роли
        /// </summary>
        protected void EnsureHasRole(string role)
        {
            EnsureAuthenticated();
            if (CurrentUserRole != role)
                throw new UnauthorizedAccessException($"User does not have required role: {role}");
        }

        /// <summary>
        /// Упрощенный метод для проверки владения ресурсом (например, patientId принадлежит пользователю)
        /// </summary>
        protected bool OwnsResource(Guid resourcePatientId)
        {
            return CurrentPatientId == resourcePatientId || IsAdmin || IsEmployee;
        }
    }
}