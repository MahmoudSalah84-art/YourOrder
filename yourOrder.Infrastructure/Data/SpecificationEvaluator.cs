using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity;
using yourOrder.Core.Specifications;

namespace yourOrder.Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery, ISpecification<T> spec)
        {

            var Query = InputQuery;

            if (spec.Criteria is not null)
                Query = Query.Where(spec.Criteria);

            if (spec.OrderBy is not null)
                Query = Query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending is not null)
                Query = Query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
                Query = Query.Skip(spec.Skip).Take(spec.Take);

            Query = spec.Includes.Aggregate(Query, (CurrentQuery, include) => CurrentQuery.Include(include)); //currentQuery is const , there are two include

            return Query;
        }
    }
}
