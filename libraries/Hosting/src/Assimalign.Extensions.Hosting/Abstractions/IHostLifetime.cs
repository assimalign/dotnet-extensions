﻿using System.Threading;
using System.Threading.Tasks;

namespace Assimalign.Extensions.Hosting.Abstractions
{
    public interface IHostLifetime
    {
        /// <summary>
        /// Called at the start of <see cref="IHost.StartAsync(CancellationToken)"/> which will wait until it's complete before
        /// continuing. This can be used to delay startup until signaled by an external event.
        /// </summary>
        /// <param name="cancellationToken">Used to abort program start.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task WaitForStartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Called from <see cref="IHost.StopAsync(CancellationToken)"/> to indicate that the host is stopping and it's time to shut down.
        /// </summary>
        /// <param name="cancellationToken">Used to indicate when stop should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task StopAsync(CancellationToken cancellationToken);
    }
}
