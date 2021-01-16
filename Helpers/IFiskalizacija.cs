using Fiskal.Model;
using System.Threading.Tasks;

namespace FiskalApp.Controllers
{
    public interface IFiskalizacija
    {
        public Task<Racun> FiskalizirajRacun(Racun racun);
        public Task<Racun> FiskalizirajRacun(int id);
    }
}