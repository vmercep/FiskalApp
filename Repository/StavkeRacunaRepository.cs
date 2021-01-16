using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Repository
{
    public class StavkeRacunaRepository : RepositoryBase<StavkeRacuna>, IStavkeRacunaRepository
    {
        public StavkeRacunaRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            //this.appDbContext = appDbContext;
        }
    }
}
