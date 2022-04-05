﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assimalign.Extensions.Configuration.Abstractions;

/// <summary>
/// Represents a section of application configuration values.
/// </summary>
public interface IConfigurationSection : IConfiguration
{
    /// <summary>
    /// Gets the key this section occupies in its parent.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the full path to this section within the <see cref="IConfiguration"/>.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets or sets the section value.
    /// </summary>
    string Value { get; set; }
}