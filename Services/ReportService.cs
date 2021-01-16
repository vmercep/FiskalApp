using FiskalApp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace FiskalApp.Services
{
    public class ReportService : IHostedService
    {
        private readonly CrontabSchedule _crontabSchedule;
        private DateTime _nextRun;
        private readonly ILogger<ReportService> _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly string networkMail;
        private readonly string networkMailPassword;


        public ReportService(IOptions<AppSettings> op, ILogger<ReportService> logger, IServiceScopeFactory scopeFactory)
        {
            _crontabSchedule = CrontabSchedule.Parse(op.Value.ReportScheduler, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            _nextRun = _crontabSchedule.GetNextOccurrence(DateTime.Now);
            _logger = logger;
            this.scopeFactory = scopeFactory;
            networkMail = op.Value.Mail;
            networkMailPassword = op.Value.Password;

        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(UntilNextExecution(), cancellationToken); // wait until next time
                    _logger.LogInformation("Task run for report");
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
                _logger.LogInformation("Running daily report");
                using (var scope = scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<Model.AppDbContext>();
                    var settings=db.Settings.FirstOrDefault();
                    var racuni = db.Racun.Where(m => m.DatumRacuna > DateTime.Now.AddDays(-1) && m.DatumRacuna <= DateTime.Now);
                    int count = racuni.Count();
                    int nefisk = racuni.Where(p => p.Jir == null).Count();


                    using (var message = new MailMessage())
                    {
                        message.To.Add(new MailAddress(settings.Email));
                        message.From = new MailAddress("fikal.service@mail.com", "WendingFiskalizacija");
                        message.Subject = "Dnevni Report";
                        message.Body = String.Format("U proteklih 24h ukupno ima {0} računa, od toga je {1} nefiskaliziranih",count, nefisk);
                        message.IsBodyHtml = true;

                        using (var client = new SmtpClient("smtp.gmail.com"))
                        {
                            client.Port = 587;
                            client.Credentials = new NetworkCredential(networkMail, networkMailPassword);
                            client.EnableSsl = true;
                            client.Send(message);
                        }
                    }

                }

            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }


            return Task.CompletedTask;
        }
    }
}
