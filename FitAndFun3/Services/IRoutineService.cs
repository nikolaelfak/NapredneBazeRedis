using FitAndFun.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAndFun.Services
{
    public interface IRoutineService
    {
        Task <Routine> GetRoutineById(int routineId);
        Task AddRoutine(Routine routine);
        Task UpdateRoutine(Routine routine);
        Task DeleteRoutine(int routineId);
        Task <IEnumerable<Routine>> GetRoutinesByUserId(int userId);
        Task<IEnumerable<Routine>> GetCachedRoutinesByUserId(int userId);

    }
}
