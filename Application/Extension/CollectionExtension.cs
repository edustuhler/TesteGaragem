using System.Linq;
using System.Linq.Expressions;

namespace Application
{
    public static class CollectionExtension
    {

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> collection, Func<T, bool> expression, bool condition)
        {
            if (condition)
            {
                return collection.Where(expression);
            }
            else
            {
                return collection;
            }
        }
    }
}