﻿using System;


namespace Assimalign.Extensions.Logging
{
    using Assimalign.Extensions.Configuration;

    /// <summary>
    /// Allows access to configuration section associated with logger provider
    /// </summary>
    public interface ILoggerProviderConfigurationFactory
    {
        /// <summary>
        /// Return configuration section associated with logger provider
        /// </summary>
        /// <param name="providerType">The logger provider type</param>
        /// <returns>The <see cref="IConfiguration"/> for the given <paramref name="providerType" />.</returns>
        IConfiguration GetConfiguration(Type providerType);
    }
}
