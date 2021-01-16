using Fiskal.Model;
using FiskalApp.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Model
{
    public interface IRacunRepository : IRepositoryBase<Racun>
    {
        Racun PopulateRacun(Racun racun);
        string GetRacunBroj();
        /*
        Task<IEnumerable<Racun>> GetAllRacun();
        Task<Racun> GetRacun(int racId);
        Task<Racun> UpdateRacun(Racun racun);

        Task<Racun> AddRacun(Racun racun);
        */
    }
}
