using System;
using System.Reflection;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// Represent property metadata
    /// </summary>
    public class PropertyMetadata : IMemberMetadata<PropertyInfo>, IInjectable, ISortedInject
    {
        /// <inheritdoc />
        public PropertyInfo Member { get; init; }

        /// <inheritdoc />
        public int Order { get; init; }
        
        /// <inheritdoc />
        public bool IsRequired { get; init; }
        
        /// <inheritdoc />
        public Type ServiceType { get; init; }
    }
}
