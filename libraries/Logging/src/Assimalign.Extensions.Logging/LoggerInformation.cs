﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assimalign.Extensions.Logging
{
    using Assimalign.Extensions.Logging;

    internal readonly struct MessageLogger
    {
        public MessageLogger(ILogger logger, string category, string providerTypeFullName, LogLevel? minLevel, Func<string, string, LogLevel, bool> filter)
        {
            Logger = logger;
            Category = category;
            ProviderTypeFullName = providerTypeFullName;
            MinLevel = minLevel;
            Filter = filter;
        }

        public ILogger Logger { get; }

        public string Category { get; }

        private string ProviderTypeFullName { get; }

        public LogLevel? MinLevel { get; }

        public Func<string, string, LogLevel, bool> Filter { get; }

        public bool IsEnabled(LogLevel level)
        {
            if (MinLevel != null && level < MinLevel)
            {
                return false;
            }

            if (Filter != null)
            {
                return Filter(ProviderTypeFullName, Category, level);
            }

            return true;
        }
    }

    internal readonly struct ScopeLogger
    {
        public ScopeLogger(ILogger logger, IExternalScopeProvider externalScopeProvider)
        {
            Logger = logger;
            ExternalScopeProvider = externalScopeProvider;
        }

        public ILogger Logger { get; }

        public IExternalScopeProvider ExternalScopeProvider { get; }

        public IDisposable CreateScope<TState>(TState state)
        {
            if (ExternalScopeProvider != null)
            {
                return ExternalScopeProvider.Push(state);
            }
            return Logger.BeginScope<TState>(state);
        }
    }

    internal readonly struct LoggerInformation
    {
        public LoggerInformation(ILoggerProvider provider, string category) : this()
        {
            ProviderType = provider.GetType();
            Logger = provider.CreateLogger(category);
            Category = category;
            ExternalScope = provider is ISupportExternalScope;
        }

        public ILogger Logger { get; }

        public string Category { get; }

        public Type ProviderType { get; }

        public bool ExternalScope { get; }
    }
}
