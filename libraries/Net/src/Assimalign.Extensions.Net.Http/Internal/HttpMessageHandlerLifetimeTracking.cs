﻿using System.Net.Http;

namespace Assimalign.Extensions.Net.Http
{
    // This a marker used to check if the underlying handler should be disposed. HttpClients
    // share a reference to an instance of this class, and when it goes out of scope the inner handler
    // is eligible to be disposed.
    internal sealed class HttpMessageHandlerLifetimeTracking : DelegatingHandler
    {
        public HttpMessageHandlerLifetimeTracking(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override void Dispose(bool disposing)
        {
            // The lifetime of this is tracked separately by ActiveHandlerTrackingEntry
        }
    }
}
