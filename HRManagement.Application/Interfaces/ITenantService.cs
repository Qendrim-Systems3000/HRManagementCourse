namespace HRManagement.Application.Interfaces
{
    public interface ITenantService
    {
        int GetCurrentTenantId();
        /// <summary>Returns null when there is no user context (e.g. during startup seeding).</summary>
        int? GetCurrentTenantIdOrNull();
    }
}