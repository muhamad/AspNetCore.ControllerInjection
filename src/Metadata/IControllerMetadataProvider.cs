using Microsoft.AspNetCore.Mvc;
using System;

namespace AspNetCore.ControllerInjection.Metadata
{
    /// <summary>
    /// provider to retrieve controller metadata
    /// </summary>
    public interface IControllerMetadataProvider
    {
        /// <summary>
        /// create controller instance
        /// </summary>
        /// <param name="controllerType">controller type</param>
        /// <returns>controller instance</returns>
        ControllerBase CreateControllerInstance(Type controllerType);
        
        /// <summary>
        /// get controller metadata or retrieve already cached ones
        /// </summary>
        /// <param name="controllerType">controller type</param>
        /// <returns>controller metadata information</returns>
        ControllerMetadata CreateOrGetMetadata(Type controllerType);
    }
}
