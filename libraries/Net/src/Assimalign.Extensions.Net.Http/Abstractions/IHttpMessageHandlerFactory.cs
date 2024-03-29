﻿using System.Net.Http;

namespace Assimalign.Extensions.Net.Http;

/// <summary>
/// A factory abstraction for a component that can create <see cref="HttpMessageHandler"/> instances with custom
/// configuration for a given logical name.
/// </summary>
public interface IHttpMessageHandlerFactory
{
    /// <summary>
    /// Creates and configures an <see cref="HttpMessageHandler"/> instance using the configuration that corresponds
    /// to the logical name specified by <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The logical name of the message handler to create.</param>
    /// <returns>A new <see cref="HttpMessageHandler"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// The default <see cref="IHttpMessageHandlerFactory"/> implementation may cache the underlying
    /// <see cref="HttpMessageHandler"/> instances to improve performance.
    /// </para>
    /// <para>
    /// The default <see cref="IHttpMessageHandlerFactory"/> implementation also manages the lifetime of the
    /// handler created, so disposing of the <see cref="HttpMessageHandler"/> returned by this method may
    /// have no effect.
    /// </para>
    /// </remarks>
    HttpMessageHandler CreateHandler(string name);
}
