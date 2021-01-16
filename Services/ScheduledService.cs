using FiskalApp.Controllers;
using FiskalApp.Helpers;
using FiskalApp.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiskalApp.Services
{
    public class ScheduledService : IHostedService
    {
        private readonly CrontabSchedule _crontabSchedule;
        private DateTime _nextRun;
        //private const string Schedule = "0 * * * *"; // run day at 1 am
        private readonly ILogger<ScheduledService> _logger;
        private readonly IServiceScopeFactory scopeFactory;

        public ScheduledService(IOptions<AppSettings> op, ILogger<ScheduledService> logger, IServiceScopeFactory scopeFactory)
        {
            _crontabSchedule = CrontabSchedule.Parse(op.Value.FiskalScheduler, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            _nextRun = _crontabSchedule.GetNextOccurrence(DateTime.Now);
            _logger = logger;
            this.scopeFactory = scopeFactory;

        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(UntilNextExecution(), cancellationToken); // wait until next time
                    _logger.LogInformation("Task run for checking not fiskal data");
                    //await _task.Execute(); //execute some task
                    await CheckBills();



                    _nextRun = _crontabSchedule.GetNextOccurrence(DateTime.Now);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private int UntilNextExecution() => Math.Max(0, (int)_nextRun.Subtract(DateTime.Now).TotalMilliseconds);

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


        private Task CheckBills()
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<Model.AppDbContext>();
                    var fiskla = scope.ServiceProvider.GetRequiredService<IFiskalizacija>();
                    var racuni = db.Racun.Where(m => m.Jir == null);
                    var listaNefiskal = new List<int>();
                    foreach (var racun in racuni)
                    {
                        _logger.LogInformation("Nefiskalizirani racun: " + racun.BrojRacuna);

                        listaNefiskal.Add(racun.Id);
                        


                    }
                    foreach (var id in listaNefiskal)
                    {
                        _logger.LogInformation("Fiskaliziram racun id "+id);
                        fiskla.FiskalizirajRacun(id);
                    }

                }

            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message);
            }


            return Task.CompletedTask;
        }
    }
}
