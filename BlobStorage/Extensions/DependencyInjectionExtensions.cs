using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStorage.Extensions
{
    public static class DependencyInjectionExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddLocalBlobStorage(string filePath)
            {
                services.AddSingleton<IBlobStorage>(_ => new LocalBlobStorage(filePath));
                services.AddSingleton<LocalBlobStorage>(s => (LocalBlobStorage)s.GetRequiredService<IBlobStorage>());
                return services;
            }
        }
    }
}
