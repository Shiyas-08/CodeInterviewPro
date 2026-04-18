using CodeInterviewPro.Application.DTOs.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(string role, Guid userId, Guid tenantId);
    Task<InsightsDto> GetInsightsAsync(string role, Guid userId, Guid tenantId);
}