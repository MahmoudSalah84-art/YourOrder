using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity
{
    public class ProductType(string name,int id): BaseEntity(id) //primary constractor in .net 9
    {
        public required string Name { get; set; } = name ;
    }
}
