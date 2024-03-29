﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Assimalign.Extensions.DependencyInjection;

using Assimalign.Extensions.DependencyInjection.ServiceLoopkup;
using Assimalign.Extensions.DependencyInjection.ServiceLoopkup.Kind;

/// <summary>
/// The default IServiceProvider.
/// </summary>
public sealed class ServiceProvider : IServiceProvider, IDisposable, IAsyncDisposable
{
    private readonly CallSiteValidator callSiteValidator;
    private readonly Func<Type, Func<ServiceProviderEngineScope, object>> createServiceAccessor;

    // Internal for testing
    internal ServiceProviderEngine engine;

    private bool isDisposed;
    private ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object>> realizedServices;

    internal CallSiteFactory CallSiteFactory { get; }
    internal ServiceProviderEngineScope Root { get; }

    internal static bool VerifyOpenGenericServiceTrimmability { get; } =
        AppContext.TryGetSwitch("Assimalign.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability", out bool verifyOpenGenerics) ? verifyOpenGenerics : false;

    internal ServiceProvider(ICollection<ServiceDescriptor> serviceDescriptors, ServiceProviderOptions options)
    {
        // note that Root needs to be set before calling GetEngine(), because the engine may need to access Root
        Root = new ServiceProviderEngineScope(this, isRootScope: true);
        engine = GetEngine();
        createServiceAccessor = CreateServiceAccessor;
        realizedServices = new ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object>>();

        CallSiteFactory = new CallSiteFactory(serviceDescriptors);
        // The list of built in services that aren't part of the list of service descriptors
        // keep this in sync with CallSiteFactory.IsService
        CallSiteFactory.Add(typeof(IServiceProvider), new ServiceProviderCallSite());
        CallSiteFactory.Add(typeof(IServiceScopeFactory), new ConstantCallSite(typeof(IServiceScopeFactory), Root));
        CallSiteFactory.Add(typeof(IServiceProviderHandler), new ConstantCallSite(typeof(IServiceProviderHandler), CallSiteFactory));

        if (options.ValidateScopes)
        {
            callSiteValidator = new CallSiteValidator();
        }
        if (options.ValidateOnBuild)
        {
            List<Exception> exceptions = null;

            foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
            {
                try
                {
                    ValidateService(serviceDescriptor);
                }
                catch (Exception exception)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(exception);
                }
            }
            if (exceptions is not null)
            {
                throw new AggregateException("Some services are not able to be constructed", exceptions.ToArray());
            }
        }

        ServiceEventSource.Log.ServiceProviderBuilt(this);
    }

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of the service to get.</param>
    /// <returns>The service that was produced.</returns>
    public object GetService(Type serviceType) => GetService(serviceType, Root);

    /// <inheritdoc />
    public void Dispose()
    {
        DisposeCore();
        Root.Dispose();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DisposeCore();
        return Root.DisposeAsync();
    }

    private void DisposeCore()
    {
        isDisposed = true;
        ServiceEventSource.Log.ServiceProviderDisposed(this);
    }
    private void OnCreate(CallSiteService callSite)
    {
        callSiteValidator?.ValidateCallSite(callSite);
    }
    private void OnResolve(Type serviceType, IServiceScope scope)
    {
        callSiteValidator?.ValidateResolution(serviceType, scope, Root);
    }
    internal object GetService(Type serviceType, ServiceProviderEngineScope serviceProviderEngineScope)
    {
        if (isDisposed)
        {
            ThrowHelper.ThrowObjectDisposedException();
        }

        Func<ServiceProviderEngineScope, object> realizedService = realizedServices.GetOrAdd(serviceType, createServiceAccessor);
        OnResolve(serviceType, serviceProviderEngineScope);
        ServiceEventSource.Log.ServiceResolved(this, serviceType);
        var result = realizedService.Invoke(serviceProviderEngineScope);
        System.Diagnostics.Debug.Assert(result is null || CallSiteFactory.IsService(serviceType));
        return result;
    }
    private void ValidateService(ServiceDescriptor descriptor)
    {
        if (descriptor.ServiceType.IsGenericType && !descriptor.ServiceType.IsConstructedGenericType)
        {
            return;
        }
        try
        {
            var callSite = CallSiteFactory.GetCallSite(descriptor, new CallSiteChain());
            
            if (callSite != null)
            {
                OnCreate(callSite);
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Error while validating the service descriptor '{descriptor}': {exception.Message}", exception);
        }
    }
    private Func<ServiceProviderEngineScope, object> CreateServiceAccessor(Type serviceType)
    {
        var callSite = CallSiteFactory.GetCallSite(serviceType, new CallSiteChain());
        if (callSite is not null)
        {
            ServiceEventSource.Log.CallSiteBuilt(this, serviceType, callSite);
            OnCreate(callSite);

            // Optimize singleton case
            if (callSite.Cache.Location == CallSiteResultCacheLocation.Root)
            {
                object value = CallSiteRuntimeResolver.Instance.Resolve(callSite, Root);
                return scope => value;
            }

            return engine.RealizeService(callSite);
        }

        return _ => null;
    }
    internal void ReplaceServiceAccessor(CallSiteService callSite, Func<ServiceProviderEngineScope, object> accessor)
    {
        realizedServices[callSite.ServiceType] = accessor;
    }
    internal IServiceScope CreateScope()
    {
        if (isDisposed)
        {
            ThrowHelper.ThrowObjectDisposedException();
        }
        return new ServiceProviderEngineScope(this, isRootScope: false);
    }
    private ServiceProviderEngine GetEngine()
    {
        ServiceProviderEngine engine;

        if (RuntimeFeature.IsDynamicCodeCompiled)
        {
            engine = new DynamicServiceProviderEngine(this);
        }
        else
        {
            // Don't try to compile Expressions/IL if they are going to get interpreted
            engine = RuntimeServiceProviderEngine.Instance;
        }
        return engine;
    }
}
