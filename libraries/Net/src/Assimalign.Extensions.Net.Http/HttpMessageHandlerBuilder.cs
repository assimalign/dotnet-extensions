﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Assimalign.Extensions.Net.Http;

/// <summary>
/// A builder abstraction for configuring <see cref="HttpMessageHandler"/> instances.
/// </summary>
/// <remarks>
/// The <see cref="HttpMessageHandlerBuilder"/> is registered in the service collection as
/// a transient service. Callers should retrieve a new instance for each <see cref="HttpMessageHandler"/> to
/// be created. Implementors should expect each instance to be used a single time.
/// </remarks>
public abstract class HttpMessageHandlerBuilder
{
    /// <summary>
    /// Gets or sets the name of the <see cref="HttpClient"/> being created.
    /// </summary>
    /// <remarks>
    /// The <see cref="Name"/> is set by the <see cref="IHttpClientFactory"/> infrastructure
    /// and is public for unit testing purposes only. Setting the <see cref="Name"/> outside of
    /// testing scenarios may have unpredictable results.
    /// </remarks>
    public abstract string Name { get; set; }

    /// <summary>
    /// Gets or sets the primary <see cref="HttpMessageHandler"/>.
    /// </summary>
    public abstract HttpMessageHandler PrimaryHandler { get; set; }

    /// <summary>
    /// Gets a list of additional <see cref="DelegatingHandler"/> instances used to configure an
    /// <see cref="HttpClient"/> pipeline.
    /// </summary>
    public abstract IList<DelegatingHandler> AdditionalHandlers { get; }

    /// <summary>
    /// Creates an <see cref="HttpMessageHandler"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="HttpMessageHandler"/> built from the <see cref="PrimaryHandler"/> and
    /// <see cref="AdditionalHandlers"/>.
    /// </returns>
    public abstract HttpMessageHandler Build();

    protected internal static HttpMessageHandler CreateHandlerPipeline(HttpMessageHandler primaryHandler, IEnumerable<DelegatingHandler> additionalHandlers)
    {
        // This is similar to https://github.com/aspnet/AspNetWebStack/blob/master/src/System.Net.Http.Formatting/HttpClientFactory.cs#L58
        // but we don't want to take that package as a dependency.

        if (primaryHandler == null)
        {
            throw new ArgumentNullException(nameof(primaryHandler));
        }

        if (additionalHandlers == null)
        {
            throw new ArgumentNullException(nameof(additionalHandlers));
        }

        IReadOnlyList<DelegatingHandler> additionalHandlersList = additionalHandlers as IReadOnlyList<DelegatingHandler> ?? additionalHandlers.ToArray();

        HttpMessageHandler next = primaryHandler;

        for (int i = additionalHandlersList.Count - 1; i >= 0; i--)
        {
            DelegatingHandler handler = additionalHandlersList[i];
            if (handler == null)
            {
                throw new InvalidOperationException();
            }

            // Checking for this allows us to catch cases where someone has tried to re-use a handler. That really won't
            // work the way you want and it can be tricky for callers to figure out.
            if (handler.InnerHandler != null)
            {
                throw new InvalidOperationException();
            }

            handler.InnerHandler = next;
            next = handler;
        }

        return next;
    }
}
