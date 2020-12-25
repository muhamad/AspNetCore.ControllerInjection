using System;
using System.Reflection;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// Represent field metadata
    /// </summary>
    public class FieldMetadata : IMemberMetadata<FieldInfo>, IInjectable, ISortedInject
    {
        /// <inheritdoc />
        public FieldInfo Member { get; init; }

        /// <inheritdoc />
        public int Order { get; init; }

        /// <inheritdoc />
        public bool IsRequired { get; init; }

        /// <inheritdoc />
        public Type ServiceType { get; init; }
    }
}
