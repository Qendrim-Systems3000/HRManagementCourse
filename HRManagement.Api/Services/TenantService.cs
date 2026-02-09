using HRManagement.Application.Interfaces;

namespace HRManagement.Api.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentTenantId()
        {
            var id = GetCurrentTenantIdOrNull();
            if (id == null) throw new InvalidOperationException("No user context available.");
            return id.Value;
        }

        public int? GetCurrentTenantIdOrNull()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;
            var claim = user.Claims.FirstOrDefault(c => c.Type == "LeaProfileId");
            if (claim == null || !int.TryParse(claim.Value, out var id)) return null;
            return id;
        }
    }
}

