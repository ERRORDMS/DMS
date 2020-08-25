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
    public class UploadFileTask 
    {
        private static List<Task> tasks;
        public UploadFileTask()
        {
            tasks = new List<Task>();
        }

        public static void AddTask(Task task)
        {
            tasks.Add(task);
        }

        public static async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (tasks.Count > 0)
                {
                    Logger.Log("Test");

                    Task task = tasks.First();
                    await task;
                    //tasks.Remove(task);
                }

            }
        }
    }
}
