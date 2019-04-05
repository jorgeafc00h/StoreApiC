
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.Context.Data
{
    public class StoreContextSeed
    {
        public async Task SeedAsync(StoreDbContext context,IHostingEnvironment env, ILogger<StoreDbContext> logger)
        {

            var policy = CreatePolicy(logger, nameof(StoreDbContext));

            // apply database migrations.
            await policy.ExecuteAsync(async ()  =>
            {

            });
        }

        /// <summary>
        /// From Polly dependency
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="prefix"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        private AsyncRetryPolicy CreatePolicy(ILogger<StoreDbContext> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
