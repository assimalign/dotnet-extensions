


using System;
using System.Collections.Generic;
using System.Text;

namespace Assimalign.Extensions.DependencyInjection.Specification.Fakes
{
    public class ClassWithServiceProvider
    {
        public ClassWithServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
