using CodeInterviewPro.Application.DTOs.Dashboard;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repo;
        private readonly ICacheService _cache;

        public DashboardService(
    IDashboardRepository repo,
    ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        //public async Task<DashboardSummaryDto> GetSummaryAsync(string role, Guid userId, Guid tenantId)
        //{
        //    return role switch
        //    {
        //        "1" => await _repo.GetAdminSummaryAsync(),
        //        "2" => await _repo.GetHrSummaryAsync(tenantId),
        //        "3" => await _repo.GetCandidateSummaryAsync(userId),
        //        _ => throw new Exception("Invalid role")
        //    };
        //}
        //public async Task<InsightsDto> GetInsightsAsync(string role, Guid userId, Guid tenantId)
        //{
        //    return await _repo.GetInsightsAsync(role, userId, tenantId);
        //}
        public async Task<DashboardSummaryDto> GetSummaryAsync(string role, Guid userId, Guid tenantId)
        {
            return role switch
            {
                "1" => await _repo.GetAdminSummaryAsync(),
                "2" => await _repo.GetHrSummaryAsync(tenantId),
                "3" => await _repo.GetCandidateSummaryAsync(userId),
                _ => throw new Exception("Invalid role")
            };
        }

        public async Task<InsightsDto> GetInsightsAsync(string role, Guid userId, Guid tenantId)
        {
            return await _repo.GetInsightsAsync(role, userId, tenantId);
        }
    }

}
