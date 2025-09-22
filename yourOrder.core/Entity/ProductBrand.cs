using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity
{
    public class ProductBrand (string name, int id) : BaseEntity(id)
    {
        public required string Name { get; set; } = name ;
    }
}
