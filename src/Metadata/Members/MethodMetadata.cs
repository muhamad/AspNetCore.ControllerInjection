using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// Represent method metadata
    /// </summary>
    public class MethodMetadata : IMemberMetadata<MethodInfo>, IRequireInjection, ISortedInject
    {
        /// <inheritdoc />
        public MethodInfo Member { get; init; }

        /// <inheritdoc />
        public int Order { get; init; }

        /// <inheritdoc />
        public bool IsRequired { get; init; }

        /// <summary>
        /// Get list of method parameters
        /// </summary>
        public IReadOnlyList<ParameterMetadata> Parameters { get; init; }

        /// <summary>
        /// get arguments for the method
        /// </summary>
        /// <param name="provider">service provider</param>
        /// <returns>an array contains method arguments</returns>
        public object[] GetParameterValues(IServiceProvider provider)
            => Parameters.Select(e => e.GetInjectableValue(provider)).ToArray();
    }

    
    /// <summary>
    /// Represent method parameter metadata
    /// </summary>
    public class ParameterMetadata : IInjectable
    {
        /// <summary>
        /// Get Parameter information
        /// </summary>
        public ParameterInfo Parameter { get; init; }
        
        /// <summary>
        /// Get whether an injected type is required
        /// </summary>
        public bool IsRequired { get; init; }
        
        /// <summary>
        /// Get type of service to inject
        /// </summary>
        public Type ServiceType { get; init; }
    }
}
