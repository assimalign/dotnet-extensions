﻿using System;

namespace Assimalign.Extensions.Hosting.Internal
{
    using Assimalign.Extensions.Hosting;
    using Assimalign.Extensions.DependencyInjection;

    internal sealed class HostServiceFactoryAdapter<TContainerBuilder> : IHostServiceFactoryAdapter
    {
        private readonly Func<HostBuilderContext> hostContextResolver;
        private IServiceProviderFactory<TContainerBuilder> serviceProviderFactory;
        private Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> serviceProviderFactoryResolver;

        public HostServiceFactoryAdapter(
            IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? 
                throw new ArgumentNullException(nameof(serviceProviderFactory));
        }


        public HostServiceFactoryAdapter(
            Func<HostBuilderContext> hostContextResolver, 
            Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> serviceProviderFactoryResolver)
        {
            this.hostContextResolver = hostContextResolver ?? 
                throw new ArgumentNullException(nameof(hostContextResolver));

            this.serviceProviderFactoryResolver = serviceProviderFactoryResolver ?? 
                throw new ArgumentNullException(nameof(serviceProviderFactoryResolver));
        }


        public object CreateBuilder(IServiceCollection services)
        {
            if (this.serviceProviderFactory == null)
            {
                serviceProviderFactory = serviceProviderFactoryResolver.Invoke(hostContextResolver.Invoke());
                if (serviceProviderFactory == null)
                {
                    throw new InvalidOperationException();// SR.ResolverReturnedNull);
                }
            }
            return serviceProviderFactory.CreateBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(object containerBuilder)
        {
            if (serviceProviderFactory == null)
            {
                throw new InvalidOperationException();// SR.CreateBuilderCallBeforeCreateServiceProvider);
            }

            return serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
        }
    }
}
