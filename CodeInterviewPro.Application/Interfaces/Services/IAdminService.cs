using CodeInterviewPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task CreateHr(CreateHrRequest request);
    }
}
