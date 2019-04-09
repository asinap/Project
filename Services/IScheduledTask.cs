using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace test2.Services
{
    interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
    public class SomeTask : IScheduledTask
    {
        public string Schedule => "5 * * * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

        }
    }
}
