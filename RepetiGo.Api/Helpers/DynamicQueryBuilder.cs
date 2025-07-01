using System.Linq.Expressions;
using System.Reflection;

namespace RepetiGo.Api.Helpers
{
    public static class DynamicQueryBuilder
    {
        public static Func<IQueryable<T>, IOrderedQueryable<T>>? BuildSortBy<T>(string? sortBy, bool isDescending = false) where T : class
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return null;
            }

            // Validate property exist
            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property is null) return null;

            return (query) =>
            {
                // Create a parameter expression of lambda, e.g. x => x.PropertyName
                var parameter = Expression.Parameter(typeof(T), "x");

                // this is the part that accesses the property (body of lambda), e.g. x.PropertyName
                var propertyAccess = Expression.Property(parameter, property);

                // this is the lambda expression, e.g. x => x.PropertyName
                var lambda = Expression.Lambda(propertyAccess, parameter);

                // Determine the method name based on whether it's descending or ascending
                var methodName = isDescending ? "OrderByDescending" : "OrderBy";

                // Get the method info for OrderBy or OrderByDescending
                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.PropertyType);

                return (IOrderedQueryable<T>)method.Invoke(null, [query, lambda])!;
            };
        }

        public static Expression<Func<T, bool>>? BuildFilter<T>(string? filterExpression) where T : class
        {
            if (string.IsNullOrEmpty(filterExpression))
            {
                return null;
            }

            // Parse the filter string like "PropertyName1=Value1,PropertyName2=Valu2,..."
            var filters = filterExpression.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Create a parameter expression for the type T, e.g. x => x.PropertyName
            var parameter = Expression.Parameter(typeof(T), "x");

            // Initialize the combined expression to null
            Expression? combinedExpression = null;

            foreach (var filter in filters)
            {
                // Split the filter into property name and value, e.g. "PropertyName=Value"
                var parts = filter.Split("=", 2);
                if (parts.Length != 2) continue;

                // Trim whitespace and get property name and value
                var propertyName = parts[0].Trim();
                var value = parts[1].Trim();

                // Validate property exists
                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // If property does not exist, skip this filter
                if (property is null) return null;

                // Create a property access expression, e.g. x.PropertyName
                var propertyAccess = Expression.Property(parameter, propertyName);

                // Handle operations ==, >, <, Contains based on property type
                Expression valueExpression;
                if (property.PropertyType == typeof(string))
                {
                    // For string properties, we use Contains
                    valueExpression = Expression.Constant(value);

                    // Get the Contains method from the string type
                    var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);

                    // Create a call expression for Contains, e.g. x.PropertyName.Contains(value)
                    var containsCall = Expression.Call(propertyAccess, containsMethod!, valueExpression);

                    // Combine the Contains call with the existing expression
                    combinedExpression = combinedExpression is null
                        ? containsCall
                        : Expression.AndAlso(combinedExpression, containsCall);
                }
                else if (property.PropertyType == typeof(int) && int.TryParse(value, out var intValue))
                {
                    valueExpression = Expression.Constant(intValue);
                    var equalExpression = Expression.Equal(propertyAccess, valueExpression);
                    combinedExpression = combinedExpression is null ? equalExpression :
                        Expression.AndAlso(combinedExpression, equalExpression);
                }
                else if (property.PropertyType == typeof(bool) && bool.TryParse(value, out var boolValue))
                {
                    valueExpression = Expression.Constant(boolValue);
                    var equalExpression = Expression.Equal(propertyAccess, valueExpression);
                    combinedExpression = combinedExpression is null ? equalExpression :
                        Expression.AndAlso(combinedExpression, equalExpression);
                }
                else if (property.PropertyType == typeof(DateTime) && DateTime.TryParse(value, out var dateValue))
                {
                    valueExpression = Expression.Constant(dateValue);
                    var equalExpression = Expression.Equal(propertyAccess, valueExpression);
                    combinedExpression = combinedExpression is null ? equalExpression :
                        Expression.AndAlso(combinedExpression, equalExpression);
                }
                else if (property.PropertyType.IsEnum && Enum.TryParse(property.PropertyType, value, true, out var enumValue))
                {
                    valueExpression = Expression.Constant(enumValue);
                    var equalExpression = Expression.Equal(propertyAccess, valueExpression);
                    combinedExpression = combinedExpression is null ? equalExpression :
                        Expression.AndAlso(combinedExpression, equalExpression);
                }
                else if (property.PropertyType == typeof(double) && double.TryParse(value, out var doubleValue))
                {
                    valueExpression = Expression.Constant(doubleValue);
                    var equalExpression = Expression.Equal(propertyAccess, valueExpression);
                    combinedExpression = combinedExpression is null ? equalExpression :
                        Expression.AndAlso(combinedExpression, equalExpression);
                }
                else
                {
                    // Unsupported type for filtering
                    return null;
                }
            }

            return combinedExpression is not null
                ? Expression.Lambda<Func<T, bool>>(combinedExpression, parameter)
                : null;
        }
    }
}
