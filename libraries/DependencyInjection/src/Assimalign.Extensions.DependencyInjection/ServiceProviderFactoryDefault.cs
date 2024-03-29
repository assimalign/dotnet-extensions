﻿using System;

namespace Assimalign.Extensions.DependencyInjection;

public class ServiceProviderFactoryDefault : IServiceProviderFactory<IServiceCollection>
{
    private readonly ServiceProviderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceProviderFactoryDefault"/> class
    /// with default options.
    /// </summary>
    public ServiceProviderFactoryDefault() : this(ServiceProviderOptions.Default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceProviderFactoryDefault"/> class
    /// with the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options to use for this instance.</param>
    public ServiceProviderFactoryDefault(ServiceProviderOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        this.options = options;
    }

    /// <inheritdoc />
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }
    /// <inheritdoc />
    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return containerBuilder.BuildServiceProvider(options);
    }

}
