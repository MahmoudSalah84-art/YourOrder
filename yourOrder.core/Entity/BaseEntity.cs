using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity
{
    public class BaseEntity(int id)
    {
        public required int Id { get; set; } = id;
    }
}
