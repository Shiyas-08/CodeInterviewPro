using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Domain.Entities;
namespace CodeInterviewPro.Application.Interfaces.Services
{
   

    public interface IQuestionService
    {
        Task<Guid> CreateAsync(CreateQuestionDto dto);

        Task<IEnumerable<Question>> GetAllAsync();

        Task<Question?> GetByIdAsync(Guid id);

        Task UpdateAsync(UpdateQuestionDto dto);

        Task DeleteAsync(Guid id);
    }
  
}
