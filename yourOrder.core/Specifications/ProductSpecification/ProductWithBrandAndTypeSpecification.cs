using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.ProductAggregate;

namespace yourOrder.Core.Specifications.ProductSpecification
{
    public class ProductWithBrandAndTypeSpecification : BaseSpecifications<Product>
    {
        public ProductWithBrandAndTypeSpecification(ProductParams productParams)
               : base(p => (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                          (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId) &&
                          (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId)
               )
        {
            Includes.Add(p => p.productBrand);
            Includes.Add(p => p.productType);
            AddOrderBy(p => p.Name);

            ApplyPagination(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
        }

        // get specified product
        public ProductWithBrandAndTypeSpecification(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.productBrand);
            Includes.Add(p => p.productType);
        }



    }
}
