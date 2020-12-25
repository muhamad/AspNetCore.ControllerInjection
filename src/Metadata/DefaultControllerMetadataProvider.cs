using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// default implementation for <see cref="IControllerMetadataProvider"/>
    /// </summary>
    public class DefaultControllerMetadataProvider : IControllerMetadataProvider
    {
        private readonly ConcurrentDictionary<Type, ObjectFactory> factories =
            new ConcurrentDictionary<Type, ObjectFactory>();
        
        private readonly ConcurrentDictionary<Type, ControllerMetadata> metadata =
            new ConcurrentDictionary<Type, ControllerMetadata>();

        private readonly IServiceProvider serviceProvider;
        
        /// <summary>
        /// initialize new instance
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        public DefaultControllerMetadataProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public ControllerBase CreateControllerInstance(Type controllerType)
        {
            if (!factories.TryGetValue(controllerType, out var factory))
            {
                factory = ActivatorUtilities.CreateFactory(controllerType, Type.EmptyTypes);
                factories.TryAdd(controllerType, factory);
            }

            return (ControllerBase)factory(serviceProvider, null);
        }

        /// <inheritdoc />
        public ControllerMetadata CreateOrGetMetadata(Type controllerType)
        {
            if (!metadata.TryGetValue(controllerType, out var meta))
            {
                var fields = GetFields(controllerType).Select(CreateFieldMetadata).ToArray();
                var props = GetProperties(controllerType).Select(CreatePropertyMetadata).ToArray();
                var methods = GetMethods(controllerType).Select(CreateMethodMetadata).ToArray();
                
                meta = new ControllerMetadata
                {
                    Fields = fields, Properties = props, Methods = methods,
                    SortedMembers = Enumerable.Empty<ISortedInject>()
                        .Concat(fields).Concat(props).Concat(methods).OrderBy(e => e.Order).ToList()
                };

                metadata.TryAdd(controllerType, meta);
            }

            return meta;
        }

        /// <summary>
        /// get fields to inject
        /// </summary>
        /// <param name="type">type to load fields from</param>
        /// <returns>a sequence of fields to inject</returns>
        protected virtual IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(e => !e.IsLiteral && !e.IsInitOnly && Attribute.IsDefined(e, typeof(InjectAttribute)));
        }

        /// <summary>
        /// create metadata for field
        /// </summary>
        /// <param name="field">field information</param>
        /// <returns>field metadata</returns>
        protected virtual FieldMetadata CreateFieldMetadata(FieldInfo field)
            => GetMetadata<FieldMetadata, FieldInfo>(field);

        /// <summary>
        /// get properties to inject
        /// </summary>
        /// <param name="type">type to load properties from</param>
        /// <returns>a sequence of properties to inject</returns>
        protected virtual IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(e => e.CanWrite && Attribute.IsDefined(e, typeof(InjectAttribute)));
        }

        /// <summary>
        /// create metadata for property
        /// </summary>
        /// <param name="property">property information</param>
        /// <returns>property metadata</returns>
        protected virtual PropertyMetadata CreatePropertyMetadata(PropertyInfo property)
            => GetMetadata<PropertyMetadata, PropertyInfo>(property);

        /// <summary>
        /// get methods for injection
        /// </summary>
        /// <param name="type">type to load methods from</param>
        /// <returns>a sequence of methods to inject</returns>
        protected virtual IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return type.GetMethods(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(IsMethodInjectable);
        }

        /// <summary>
        /// create metadata for method
        /// </summary>
        /// <param name="method">method information</param>
        /// <returns>method metadata</returns>
        protected virtual MethodMetadata CreateMethodMetadata(MethodInfo method)
        {
            var inject = method.GetCustomAttribute<InjectAttribute>();
            var order = method.GetCustomAttribute<InjectOrderAttribute>();
            
            var paramsMetadata = method.GetParameters().Select(e =>
            {
                var paramInject = e.GetCustomAttribute<InjectAttribute>();
                Type serviceType;

                if (paramInject?.ServiceType == null)
                {
                    var result = Nullable.GetUnderlyingType(e.ParameterType);
                    serviceType = result ?? e.ParameterType;
                }
                else
                    serviceType = paramInject.ServiceType;

                return new ParameterMetadata
                {
                    Parameter = e,
                    IsRequired = paramInject?.IsRequired ?? inject.IsRequired,
                    ServiceType = serviceType
                };
            }).ToArray();
            
            
            return new MethodMetadata
            {
                Member = method,
                Order = order?.Order ?? int.MaxValue,
                IsRequired = inject.IsRequired,
                Parameters = paramsMetadata
            };
        }

        /// <summary>
        /// determine whether a method is injectable
        /// </summary>
        /// <param name="method">method to check</param>
        /// <returns>true if method is injectable; false otherwise</returns>
        protected bool IsMethodInjectable(MethodInfo method)
        {
            var inject = method.GetCustomAttribute<InjectAttribute>();
            
            // a method must be marked with inject attribute, and without service type
            // a service type is for members only and the IsRequired property value will be
            // used as default value for all method parameters unless overriden for the parameter
            if (inject == null || inject.ServiceType != null) return false;

            // a method must have void return type
            if (method.ReturnType != typeof(void)) return false;

            var @params = method.GetParameters();

            // a method must have at least one parameter and no value type parameters
            if (@params.Length == 0 || @params.Count(e => !e.ParameterType.IsValueType) == 0)
                return false;

            return true;
        }

        /// <summary>
        /// get metadata for fields and properties
        /// </summary>
        /// <typeparam name="T">metadata model</typeparam>
        /// <typeparam name="U">member info type</typeparam>
        /// <param name="member">the member info object</param>
        /// <returns>the metadata object</returns>
        private static T GetMetadata<T, U>(U member)
            where U : MemberInfo
            where T : IMemberMetadata<U>, IInjectable, ISortedInject, new()
        {
            var inject = member.GetCustomAttribute<InjectAttribute>();
            var order = member.GetCustomAttribute<InjectOrderAttribute>();

            return new T
            {
                Member = member,
                Order = order?.Order ?? int.MaxValue,
                IsRequired = inject.IsRequired,
                ServiceType = inject.ServiceType ?? member switch
                {
                    FieldInfo field => field.FieldType,
                    PropertyInfo property => property.PropertyType,
                    _ => throw new Exception("member type is unknown")
                }
            };
        }
    }
}
