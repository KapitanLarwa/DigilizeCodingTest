using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digilize.Domain.Entities
{
    public class Company : EntityBase
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
    }
}
