using Fiskal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Contracts
{
    public interface IArtikliRepository : IRepositoryBase<Artikli>
    {
        Task<Artikli> UpdateArtikl(Artikli artikl);
        Task InsertArtikl(Artikli artikl);
    }
}
