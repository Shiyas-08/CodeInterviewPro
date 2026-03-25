using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs
{
    public class CreateTenantRequest
    {
        public string Name { get; set; }

        public string Domain { get; set; }
    }
}
