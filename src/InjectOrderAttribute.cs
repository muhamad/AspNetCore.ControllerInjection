using System;
using AspNetCore.ControllerInjection.Metadata;

namespace AspNetCore.ControllerInjection
{
    /// <summary>
    /// specify order of injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
    public class InjectOrderAttribute : Attribute, ISortedInject
    {
        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="order">injection order</param>
        public InjectOrderAttribute(int order)
            => Order = order;
        
        /// <summary>
        /// Get injection order
        /// </summary>
        public int Order { get; init; }
    }
}
