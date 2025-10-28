using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.ProductAggregate;

namespace yourOrder.Core.Specifications.ProductSpecification
{
    public class ProductWithFilterForCountSpecification : BaseSpecifications<Product>
    {
        public ProductWithFilterForCountSpecification(ProductParams productParams)
            : base(p =>
                        (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                        (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId) &&
                        (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId)
            )
        {
        }
    }
}
