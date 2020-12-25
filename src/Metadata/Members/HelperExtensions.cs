using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// extension methods to support injection
    /// </summary>
    internal static class HelperExtensions
    {
        /// <summary>
        /// get value of <see cref="IInjectable"/> item
        /// </summary>
        /// <param name="provider">service provider to get value from</param>
        /// <param name="injectable">injectable item</param>
        /// <returns>value for injectable item</returns>
        public static object GetInjectableValue(this IInjectable injectable, IServiceProvider provider)
        {
            if (injectable.ServiceType.IsExplicitEnumerable())
                return provider.GetServices(injectable.ServiceType);

            if (injectable.IsRequired)
                return provider.GetService(injectable.ServiceType);

            return provider.GetRequiredService(injectable.ServiceType);
        }
        
        /// <summary>
        /// determine whether input type is an <see cref="IEnumerable{T}"/> or an instantiation of it
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if input type is <see cref="IEnumerable{T}"/> or direct instantiation of it; false otherwise</returns>
        public static bool IsExplicitEnumerable(this Type type)
        {
            if (type.IsGenericTypeDefinition && type == typeof(IEnumerable<>))
                return true;

            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return true;

            return false;
        }
    }
}
