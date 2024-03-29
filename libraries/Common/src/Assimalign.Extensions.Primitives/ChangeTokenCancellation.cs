﻿using System;
using System.Threading;

namespace Assimalign.Extensions.Primitives;


/// <summary>
/// A <see cref="IChangeToken"/> implementation using <see cref="CancellationToken"/>.
/// </summary>
public class ChangeTokenCancellation : IChangeToken
{
    /// <summary>
    /// Initializes a new instance of <see cref="ChangeTokenCancellation"/>.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    public ChangeTokenCancellation(CancellationToken cancellationToken)
    {
        Token = cancellationToken;
    }

    /// <inheritdoc />
    public bool ActiveChangeCallbacks { get; private set; } = true;

    /// <inheritdoc />
    public bool HasChanged => Token.IsCancellationRequested;

    private CancellationToken Token { get; }

    /// <inheritdoc />
    public IDisposable RegisterChangeCallback(Action<object> callback, object state)
    {

        try
        {
            return Token.UnsafeRegister(callback, state);
        }
        catch (ObjectDisposedException)
        {
            // Reset the flag so that we can indicate to future callers that this wouldn't work.
            ActiveChangeCallbacks = false;
        }
        return NullDisposable.Instance;
    }

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new NullDisposable();

        public void Dispose()
        {
        }
    }
}
