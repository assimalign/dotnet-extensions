﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assimalign.Extensions.Logging;

using Assimalign.Extensions.DependencyInjection;

/// <summary>
/// An interface for configuring logging providers.
/// </summary>
public interface ILoggingBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where Logging services are configured.
    /// </summary>
    IServiceCollection Services { get; }
}
