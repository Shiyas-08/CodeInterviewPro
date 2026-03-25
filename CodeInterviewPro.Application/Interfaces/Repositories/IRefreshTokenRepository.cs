using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken token);

        Task<RefreshToken?> GetByTokenAsync(string token);

        Task RevokeAsync(string token);

    }
}
