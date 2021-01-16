using FiskalApp.Contracts;
using FiskalApp.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private AppDbContext _repoContext;
        private IRacunRepository _racun;
        private IStavkeRacunaRepository _stavkerRacuna;
        private ILogger<RacunRepository> _logger;
        public IRacunRepository Racun
        {
            get
            {
                if (_racun == null)
                {
                    _racun = new RacunRepository(_repoContext, _logger);
                }
                return _racun;
            }
        }
        public IStavkeRacunaRepository StavkeRacuna
        {
            get
            {
                if (_stavkerRacuna == null)
                {
                    _stavkerRacuna = new StavkeRacunaRepository(_repoContext);
                }
                return _stavkerRacuna;
            }
        }
        public RepositoryWrapper(AppDbContext repositoryContext, ILogger<RacunRepository> logger)
        {
            _repoContext = repositoryContext;
            _logger = logger;
        }
        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
