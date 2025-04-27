using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.DL.HelperClasses
{
    public static class KendoPaginationExtension
    {
        public static IQueryable<T> ApplyFiters<T>(this IQueryable<T> query, IList<IFilterDescriptor> filters)
        {
            if (filters != null && filters.Any())
            {
                query = query.Where(ExpressionBuilder.Expression<T>(filters));
            }
            return query;
        }

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, IList<SortDescriptor> sorts)
        {
            if (sorts != null && sorts.Any())
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var sortExpressions = sorts.Select(sort =>
                {
                    var member = Expression.PropertyOrField(parameter, sort.Member);
                    var sortType = typeof(QueryableExtensions).GetMethod(sort.SortDirection.ToString(), new Type[] { typeof(IQueryable<>), typeof(Expression<>) })
                                                              .MakeGenericMethod(typeof(T), member.Type);
                    return (Expression)Expression.Call(sortType, query.Expression, Expression.Lambda(member, parameter));
                });
                var orderedQuery = query;
                foreach (var sortExpression in sortExpressions)
                {
                    orderedQuery = orderedQuery.Provider.CreateQuery<T>(sortExpression);
                }
            }
            return query;
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page > 0 && pageSize > 0)
            {
                query = query.OrderBy(x => 1).Skip((page - 1) * pageSize).Take(pageSize);
            }
            return query;
        }
    }
}
