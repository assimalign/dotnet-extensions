﻿using System;

namespace Assimalign.Extensions.DependencyInjection;

/// <summary>
/// The <see cref="System.IDisposable.Dispose"/> method ends the scope lifetime. Once Dispose
/// is called, any scoped services that have been resolved from
/// <see cref="Assimalign.Extensions.DependencyInjection.Abstractions.IServiceScope.ServiceProvider"/> will be
/// disposed.
/// </summary>
public interface IServiceScope : IDisposable
{
    /// <summary>
    /// The <see cref="System.IServiceProvider"/> used to resolve dependencies from the scope.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}
