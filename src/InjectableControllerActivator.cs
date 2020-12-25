using AspNetCore.ControllerInjection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;

namespace AspNetCore.ControllerInjection
{
    /// <summary>
    /// provide a method/property injection for controller, also assign the required features to http context
    /// </summary>
    /// <remarks>
    /// This controller activator work in the following steps:
    ///   1. create controller instance using application service provider.
    ///   2. get all controller injectable items and fill.
    /// </remarks>
    public class InjectableControllerActivator : IControllerActivator
    {
        private readonly IControllerMetadataProvider metadataProvider;

        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="metadataProvider">controller metadata provider</param>
        public InjectableControllerActivator(IControllerMetadataProvider metadataProvider)
        {
            this.metadataProvider = metadataProvider;
        }

        /// <inheritdoc />
        public object Create(ControllerContext context)
        {
            var controller = CreateInstance(context);

            InjectElements(controller);

            return controller;
        }

        /// <inheritdoc />
        public void Release(ControllerContext context, object controller)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            (controller as IDisposable)?.Dispose();
        }

        /// <summary>
        /// execute step 1
        /// </summary>
        /// <param name="context">controller context</param>
        /// <returns>controller instance</returns>
        protected virtual ControllerBase CreateInstance(ControllerContext context)
        {
            var controller = metadataProvider.CreateControllerInstance(context.ActionDescriptor.ControllerTypeInfo);
            controller.ControllerContext = context;

            return controller;
        }

        /// <summary>
        /// execute step 2
        /// </summary>
        /// <param name="controller">controller instance</param>
        protected virtual void InjectElements(ControllerBase controller)
        {
            var metadata = metadataProvider.CreateOrGetMetadata(controller.GetType());
            var provider = controller.HttpContext.RequestServices;
            
            //TODO: optimize setting field/property value, and calling method
            
            foreach (var member in metadata.SortedMembers)
            {
                switch (member)
                {
                    case FieldMetadata field:
                        field.Member.SetValue(controller, field.GetInjectableValue(provider));
                        break;
                    case PropertyMetadata property:
                        property.Member.SetValue(controller, property.GetInjectableValue(provider));
                        break;
                    case MethodMetadata method:
                        method.Member.Invoke(controller, method.GetParameterValues(provider));
                        break;
                }
            }
        }
    }
}
