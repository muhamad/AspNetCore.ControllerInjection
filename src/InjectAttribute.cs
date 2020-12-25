using System;
using AspNetCore.ControllerInjection.Metadata;

namespace AspNetCore.ControllerInjection
{
    /// <summary>
    /// a marker attribute for runtime injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter |
                    AttributeTargets.Field)]
    public sealed class InjectAttribute : Attribute, IInjectable
    {
        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="isRequired">determine whether the service is required</param>
        public InjectAttribute(bool isRequired = false)
            => IsRequired = isRequired;

        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="serviceType">the service type to match</param>
        /// <param name="isRequired">determine whether the service is required</param>
        public InjectAttribute(Type serviceType, bool isRequired = false) : this(isRequired)
            => ServiceType = serviceType;

        /// <summary>
        /// Determine whether the service injection is required
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// The service type to match
        /// </summary>
        public Type ServiceType { get; init; }
    }
}
