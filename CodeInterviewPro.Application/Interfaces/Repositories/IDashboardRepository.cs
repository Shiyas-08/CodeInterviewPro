using CodeInterviewPro.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetAdminSummaryAsync();
        Task<DashboardSummaryDto> GetHrSummaryAsync(Guid tenantId);
        Task<DashboardSummaryDto> GetCandidateSummaryAsync(Guid userId);
        Task<InsightsDto> GetInsightsAsync(string role, Guid userId, Guid tenantId);
    }
}
