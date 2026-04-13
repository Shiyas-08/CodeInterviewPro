using CodeInterviewPro.Application.AI;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Common
{
    public class AICombinedCache
    {
        public AIIntelligenceResult AI { get; set; }
        public CodeBertResult Deep { get; set; }
        public CodeBertResult CodeBert { get; set; }
        public CodeSimilarityResult Similarity { get; set; }
    }
}
