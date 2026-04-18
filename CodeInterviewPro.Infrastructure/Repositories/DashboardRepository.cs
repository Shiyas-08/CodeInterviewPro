using CodeInterviewPro.Application.DTOs.Dashboard;
using CodeInterviewPro.Application.Interfaces.Repositories;
using Dapper;
using System.Data;

public class DashboardRepository : IDashboardRepository
{
    private readonly IDbConnection _db;

    public DashboardRepository(IDbConnection db)
    {
        _db = db;
    }

    // ADMIN
    public async Task<DashboardSummaryDto> GetAdminSummaryAsync()
    {
        var sql = @"
        SELECT 
            (SELECT COUNT(*) FROM Interviews) AS TotalInterviews,
            (SELECT COUNT(*) FROM Users WHERE Role = 3) AS TotalCandidates,
            (SELECT COUNT(*) FROM InterviewSessions WHERE Status = 2) AS CompletedInterviews,
            (SELECT ISNULL(AVG(AIScore),0) FROM ExecutionHistory) AS AverageScore
        ";

        var result = await _db.QueryFirstAsync<DashboardSummaryDto>(sql);

        result.SuccessRate = result.TotalInterviews == 0 ? 0 :
            (double)result.CompletedInterviews / result.TotalInterviews * 100;

        return result;
    }

    // HR (Tenant)
    public async Task<DashboardSummaryDto> GetHrSummaryAsync(Guid tenantId)
    {
        var sql = @"
    SELECT 
        COUNT(DISTINCT I.Id) AS TotalInterviews,
        COUNT(DISTINCT U.Id) AS TotalCandidates,
        COUNT(DISTINCT ISess.Id) AS CompletedInterviews,
        ISNULL(AVG(EH.AIScore),0) AS AverageScore
    FROM Interviews I
    LEFT JOIN Users U ON U.TenantId = I.TenantId AND U.Role = 3
    LEFT JOIN InterviewSessions ISess ON ISess.InterviewId = I.Id AND ISess.Status = 2
    LEFT JOIN ExecutionHistory EH ON EH.InterviewId = I.Id
    WHERE I.TenantId = @TenantId
    ";
        Console.WriteLine($"TENANT FROM API: {tenantId}");
        var result = await _db.QueryFirstAsync<DashboardSummaryDto>(sql, new { TenantId = tenantId });

        result.SuccessRate = result.TotalInterviews == 0 ? 0 :
            (double)result.CompletedInterviews / result.TotalInterviews * 100;

        return result;
    }

    //  Candidate
    public async Task<DashboardSummaryDto> GetCandidateSummaryAsync(Guid userId)
    {
        var sql = @"
        SELECT 
            (SELECT COUNT(DISTINCT InterviewId) FROM ExecutionHistory WHERE CandidateId = @UserId) AS TotalInterviews,
            0 AS TotalCandidates,
            (SELECT COUNT(*) FROM InterviewSessions WHERE CandidateId = @UserId AND Status = 2) AS CompletedInterviews,
            (SELECT ISNULL(AVG(AIScore),0) FROM ExecutionHistory WHERE CandidateId = @UserId) AS AverageScore
        ";

        var result = await _db.QueryFirstAsync<DashboardSummaryDto>(sql, new { UserId = userId });

        result.SuccessRate = result.TotalInterviews == 0 ? 0 :
            (double)result.CompletedInterviews / result.TotalInterviews * 100;

        return result;
    }

    public async Task<InsightsDto> GetInsightsAsync(string role, Guid userId, Guid tenantId)
    {
        string filter = "";
        object param = new();

        // 🔹 Role-based filter
        if (role == "2") // HR
        {
            filter = "JOIN Interviews I ON EH.InterviewId = I.Id WHERE I.TenantId = @TenantId";
            param = new { TenantId = tenantId };
        }
        else if (role == "3") // Candidate
        {
            filter = "WHERE EH.CandidateId = @UserId";
            param = new { UserId = userId };
        }

        // 🔹 Average Score
        var avgScoreSql = $@"
        SELECT ISNULL(AVG(AIScore),0)
        FROM ExecutionHistory EH
        {filter}
        ";

        var avgScore = await _db.ExecuteScalarAsync<double>(avgScoreSql, param);

        // 🔹 Most Common Complexity
        var complexitySql = $@"
        SELECT TOP 1 AIComplexity
        FROM ExecutionHistory EH
        {filter}
        GROUP BY AIComplexity
        ORDER BY COUNT(*) DESC
        ";

        var complexity = await _db.ExecuteScalarAsync<string>(complexitySql, param);

        // 🔹 Weaknesses (lowest scores)
        var weakSql = $@"
        SELECT TOP 3 AIFeedback
        FROM ExecutionHistory EH
        {filter}
        ORDER BY AIScore ASC
        ";

        var weaknesses = (await _db.QueryAsync<string>(weakSql, param)).ToList();

        // 🔹 Strengths (highest scores)
        var strongSql = $@"
        SELECT TOP 3 AIFeedback
        FROM ExecutionHistory EH
        {filter}
        ORDER BY AIScore DESC
        ";

        var strengths = (await _db.QueryAsync<string>(strongSql, param)).ToList();

        return new InsightsDto
        {
            AverageScore = avgScore,
            AverageComplexity = complexity ?? "N/A",
            Strengths = strengths,
            Weaknesses = weaknesses
        };
    }
}