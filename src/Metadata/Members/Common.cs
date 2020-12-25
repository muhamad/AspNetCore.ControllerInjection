using System;
using System.Reflection;

namespace AspNetCore.ControllerInjection.Metadata
{
    public interface IRequireInjection
    {
        /// <summary>
        /// Get whether an injected type is required
        /// </summary>
        bool IsRequired { get; init; }
    }
    
    /// <summary>
    /// represent an item with injection information
    /// </summary>
    public interface IInjectable : IRequireInjection
    {
        /// <summary>
        /// Get type of service to inject
        /// </summary>
        Type ServiceType { get; init; }
    }
    
    /// <summary>
    /// represent ability to inject in order
    /// </summary>
    public interface ISortedInject
    {
        /// <summary>
        /// Get order of injection
        /// </summary>
        int Order { get; init; }
    }
    
    /// <summary>
    /// represent member information
    /// </summary>
    public interface IMemberMetadata<T> where T : MemberInfo
    {
        /// <summary>
        /// Get core member information
        /// </summary>
        T Member { get; init; }
    }
}
