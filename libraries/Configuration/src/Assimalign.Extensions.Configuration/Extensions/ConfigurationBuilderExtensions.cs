﻿using System;
using System.Collections.Generic;

namespace Assimalign.Extensions.Configuration;

using Assimalign.Extensions.Configuration.Providers;

public static partial class ConfigurationBuilderExtensions
{
    #region Chaining Provider
    /// <summary>
    /// Adds an existing configuration to <paramref name="configurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="config">The <see cref="IConfiguration"/> to add.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder configurationBuilder, IConfiguration config)
        => AddConfiguration(configurationBuilder, config, shouldDisposeConfiguration: false);

    /// <summary>
    /// Adds an existing configuration to <paramref name="configurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="config">The <see cref="IConfiguration"/> to add.</param>
    /// <param name="shouldDisposeConfiguration">Whether the configuration should get disposed when the configuration provider is disposed.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder configurationBuilder, IConfiguration config, bool shouldDisposeConfiguration)
    {
        if (configurationBuilder == null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        configurationBuilder.Add(new ConfigurationChainedSource()
        {
            Configuration = config,
            ShouldDisposeConfiguration = shouldDisposeConfiguration,
        });
        return configurationBuilder;
    }
    #endregion

    #region Memory Provider
    /// <summary>
    /// Adds the memory configuration provider to <paramref name="configurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder configurationBuilder)
    {
        if (configurationBuilder == null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        configurationBuilder.Add(new ConfigurationMemorySource());
        return configurationBuilder;
    }

    /// <summary>
    /// Adds the memory configuration provider to <paramref name="configurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="initialData">The data to add to memory configuration provider.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddInMemoryCollection(
        this IConfigurationBuilder configurationBuilder,
        IEnumerable<KeyValuePair<string, string>> initialData)
    {
        if (configurationBuilder == null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        configurationBuilder.Add(new ConfigurationMemorySource { InitialData = initialData });
        return configurationBuilder;
    }
    #endregion

}
