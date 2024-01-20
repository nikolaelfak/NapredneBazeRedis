using FitAndFun.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAndFun.Services
{
    public interface IActivityService
    {
        Task<Activity> GetActivityById(int activityId);
        Task AddActivity(Activity activity);
        Task UpdateActivity(Activity activity);
        Task DeleteActivity(int activityId);
        Task<IEnumerable<Activity>> GetActivitiesByUserId(int userId);
        Task<IEnumerable<Activity>> GetAllActivities();
        Task<IEnumerable<Activity>> GetCachedActivitiesByUserId(int userId);
        Task AddToCacheAsync(string key, object value);

        // Task SubscribeToChannelAsync(string channel, System.Action<string, string> handler);
        // Task PublishMessageAsync(string channel, string message);
        // Task SetSessionAsync(string sessionId, object sessionData);
        // Task<T> GetSessionAsync<T>(string sessionId);
    }
}
