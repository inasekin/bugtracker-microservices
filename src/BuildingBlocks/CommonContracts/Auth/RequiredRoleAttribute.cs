using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommonContracts.Auth
{
  /// <summary>
  /// Атрибут для проверки роли пользователя в других микросервисах
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class RequiredRoleAttribute : Attribute, IAuthorizationFilter
  {
    private readonly string[] _requiredRoles;

    public RequiredRoleAttribute(params string[] roles)
    {
      _requiredRoles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      // Получаем роль из claims
      var roleClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "role");
      if (roleClaim == null)
      {
        context.Result = new UnauthorizedResult();
        return;
      }

      var userRole = roleClaim.Value;
      if (!_requiredRoles.Contains(userRole))
      {
        context.Result = new ForbidResult();
      }
    }
  }
}
