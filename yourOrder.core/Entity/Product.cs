using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity
{
    public class Product(int id): BaseEntity(id)
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required string PictureUrl { get; set; }

        public required ProductBrand productBrand { get; set; }
        public required int ProductBrandId { get; set; }
        public required ProductType productType { get; set; }
        public required int ProductTypeId { get; set; }

    }
}
