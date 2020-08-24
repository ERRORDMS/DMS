using DMS.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DMS
{
    public class UploadFileTask : IHostedService
    {
        private List<Task> tasks;
        private bool started;
        public UploadFileTask()
        {
            tasks = new List<Task>();
        }

        public void AddTask(Task task)
        {
            tasks.Add(task);
            if (!started)
            {
                started = true;
                Task.Run(async () => { await StartAsync(CancellationToken.None); });
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (tasks.Count > 0)
                {

                    Task task = tasks.First();
                    await task;
                    tasks.Remove(task);
                }

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            tasks.Clear();
            return Task.CompletedTask;
        }
    }
}
