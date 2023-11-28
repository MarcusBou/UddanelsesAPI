using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModels
{
    public class DTOAssignment
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string? Video { get; set; }
        public string Type { get; set; }
    }
}
