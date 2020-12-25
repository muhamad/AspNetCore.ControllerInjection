using System.Collections.Generic;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// represent controller metadata information
    /// </summary>
    public class ControllerMetadata
    {
        /// <summary>
        /// Get Fields
        /// </summary>
        public IReadOnlyList<FieldMetadata> Fields { get; init; }
        
        /// <summary>
        /// Get properties
        /// </summary>
        public IReadOnlyList<PropertyMetadata> Properties { get; init; }

        /// <summary>
        /// Get methods
        /// </summary>
        public IReadOnlyList<MethodMetadata> Methods { get; init; }

        /// <summary>
        /// Get members sorted by injection order
        /// </summary>
        public IReadOnlyList<object> SortedMembers { get; init; }
    }
}
