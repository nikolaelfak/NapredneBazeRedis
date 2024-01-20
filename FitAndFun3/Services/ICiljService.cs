using FitAndFun.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAndFun.Services
{
    public interface ICiljService
    {
        Task <Cilj> GetCiljById(int ciljId);
        Task AddCilj(Cilj cilj);
        Task UpdateCilj(Cilj cilj);
        Task DeleteCilj(int ciljId);
        Task <IEnumerable<Cilj>> GetCiljeviByUserId(int userId);
        Task<IEnumerable<Cilj>> GetCachedCiljeviByUserId(int userId);    
    }
}
