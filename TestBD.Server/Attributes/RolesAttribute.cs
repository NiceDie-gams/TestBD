using Microsoft.AspNetCore.Authorization;

namespace TestBD.Server.Attributes
{
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }

    // Конкретные атрибуты для удобства
    public class AdminOnlyAttribute : RolesAttribute
    {
        public AdminOnlyAttribute() : base("admin") { }
    }

    public class EmployeeOrAdminAttribute : RolesAttribute
    {
        public EmployeeOrAdminAttribute() : base("admin", "employee") { }
    }

    public class UserOnlyAttribute : RolesAttribute
    {
        public UserOnlyAttribute() : base("user") { }
    }
}