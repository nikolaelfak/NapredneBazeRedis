using FitAndFun.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAndFun.Services
{
    public interface INagradeService
    {
        Task <Nagrade> GetNagradeById(int nagradeId);
        Task AddNagrade(Nagrade nagrade);
        Task UpdateNagrade(Nagrade nagrade);
        Task DeleteNagrade(int nagradeId);
        Task <IEnumerable<Nagrade>> GetNagradeByUserId(int userId);
        Task<IEnumerable<Nagrade>> GetCachedNagradeByUserId(int userId);
    }
}
