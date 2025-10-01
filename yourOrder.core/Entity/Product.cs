using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity
{
    public class Product: BaseEntity
    {
        public  string Name { get; set; }
        public  string Description { get; set; }
        public  decimal Price { get; set; }
        public  string PictureUrl { get; set; }

        public  ProductBrand productBrand { get; set; }
        public  int ProductBrandId { get; set; }
        public  ProductType productType { get; set; }
        public  int ProductTypeId { get; set; }

    }
}
