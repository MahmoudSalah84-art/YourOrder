using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity;

namespace yourOrder.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get ; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>>? OrderBy { get; set; }
        public Expression<Func<T, object>>? OrderByDescending { get; set; }
        public int Take { get ; set; }
        public int Skip { get; set; }
        public bool IsPagingEnabled { get; set; }
        public BaseSpecifications() { }
        public BaseSpecifications(Expression<Func<T, bool>> ExpressionCriteria)=>this.Criteria = ExpressionCriteria;
        public void AddOrderBy(Expression<Func<T, object>> OrderByExpression) => this.OrderBy = OrderByExpression;
        public void AddOrderByDescending(Expression<Func<T, object>> OrderByDescExpression) => this.OrderByDescending = OrderByDescExpression;
        public void ApplyPagination(int skip, int take) => (IsPagingEnabled, Skip, Take) = (true, skip, take);
    }
}
