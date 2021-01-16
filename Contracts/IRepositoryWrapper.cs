using FiskalApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Contracts
{
    public interface IRepositoryWrapper
    {
        IRacunRepository Racun { get; }
        IStavkeRacunaRepository StavkeRacuna { get; }
        void Save();
    }
}
