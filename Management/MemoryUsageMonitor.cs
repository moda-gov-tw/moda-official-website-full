using Management.ManagementUtility;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Management
{
    public class MemoryUsageMonitor : IHostedService, IDisposable
    {
        static Timer _timer;
       

        public MemoryUsageMonitor()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(30)
                );
            return Task.CompletedTask;
        }

        private int execCount = 0;

        public void DoWork(object state)
        {
            //利用 Interlocked 計數防止重複執行
            Interlocked.Increment(ref execCount);
            if (execCount == 1)
            {
                try
                {
                    CommonUtility.CheckSchedule();
                }
                catch (Exception ex)
                {
                }
            }
            Interlocked.Decrement(ref execCount);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            //調整Timer為永不觸發，停用定期排程
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
