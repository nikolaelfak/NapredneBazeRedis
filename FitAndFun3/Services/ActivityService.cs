using FitAndFun.Models;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using FitAndFun.Common;

namespace FitAndFun.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IDistributedCache _cache;
        private readonly IRedisDatabase _redisDatabase;

        public ActivityService(IDistributedCache cache, IRedisDatabase redisDatabase)
        {
            _cache = cache;
            _redisDatabase = redisDatabase;
        }

        public async Task<Activity> GetActivityById(int activityId)
        {
            var cacheKey = $"Activity_{activityId}";

            try
            {
                var cachedActivity = await GetFromCacheAsync(cacheKey);

                if (cachedActivity == null)
                {
                    var activityFromDatabase = await GetActivityFromDatabaseAsync(activityId);

                    if (activityFromDatabase != null)
                    {
                        await AddToCacheAsync(cacheKey, activityFromDatabase);
                    }

                    return activityFromDatabase;
                }

                return JsonSerializer.Deserialize<Activity>(cachedActivity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobijanja aktivnosti po ID iz keša: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Activity>> GetCachedActivitiesByUserId(int userId)
        {
            var cacheKey = $"Activities_{userId}";

            try
            {
                var cachedActivities = await GetFromCacheAsync(cacheKey);

                if (cachedActivities == null)
                {
                    var activitiesFromDatabase = await GetActivitiesFromDatabaseByUserIdAsync(userId);

                    if (activitiesFromDatabase != null)
                    {
                        await AddToCacheAsync(cacheKey, activitiesFromDatabase);
                    }

                    return activitiesFromDatabase;
                }

                return JsonSerializer.Deserialize<IEnumerable<Activity>>(cachedActivities);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobijanja aktivnosti po ID iz keša: {ex.Message}");
                throw;
            }
        }

        public async Task AddActivity(Activity activity)
        {
            try
            {
                await AddActivityToDatabaseAsync(activity);
                await InvalidateCacheAsync($"Activities_{activity.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja aktivnosti: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateActivity(Activity activity)
        {
            try
            {
                await UpdateActivityInDatabaseAsync(activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja aktivnosti: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteActivity(int activityId)
        {
            try
            {
                await DeleteActivityFromDatabaseAsync(activityId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja aktivnosti sa ID {activityId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByUserId(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<IEnumerable<Activity>>($"Activities_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja aktivnosti iz Redis baze po ID korisnika: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Activity>> GetAllActivities()
        {
            try
            {
                var redisKey = "AllActivities";
                var cachedActivities = await _redisDatabase.GetAsync<IEnumerable<Activity>>(redisKey);

                if (cachedActivities == null)
                {
                    var activitiesFromDatabase = await GetAllActivitiesFromDatabaseAsync();

                    if (activitiesFromDatabase != null)
                    {
                        await AddToCacheAsync(redisKey, activitiesFromDatabase);
                    }

                    return activitiesFromDatabase;
                }

                return cachedActivities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobijanja svih aktivnosti iz Redis baze: {ex.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Activity>> GetAllActivitiesFromDatabaseAsync()
        {
            try
            {
                var activityKeys = await _redisDatabase.GetAllKeysAsync("Activity_*");

                var activities = new List<Activity>();
                foreach (var key in activityKeys)
                {
                    var activity = await _redisDatabase.GetAsync<Activity>(key);
                    if (activity != null)
                    {
                        activities.Add(activity);
                    }
                }

                return activities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobijanja svih aktivnosti iz baze podataka: {ex.Message}");
                throw;
            }
        }

        public async Task AddToCacheAsync(string key, object value)
        {
            try
            {
                var jsonValue = JsonSerializer.Serialize(value);
                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };

                await _cache.SetStringAsync(key, jsonValue, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja u keš: {ex.Message}");
                throw;
            }
        }

        private async Task InvalidateCacheAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja iz keša: {ex.Message}");
                throw;
            }
        }

        private async Task<Activity> GetActivityFromDatabaseAsync(int activityId)
        {
            try
            {
                return await _redisDatabase.GetAsync<Activity>($"Activity_{activityId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja aktivnosti iz Redis baze po ID: {ex.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Activity>> GetActivitiesFromDatabaseByUserIdAsync(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<IEnumerable<Activity>>($"Activities_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja aktivnosti iz Redis baze po ID korisnika: {ex.Message}");
                throw;
            }
        }

        private async Task AddActivityToDatabaseAsync(Activity activity)
        {
            try
            {
                await _redisDatabase.SetAsync($"Activity_{activity.ActivityId}", activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja aktivnosti u Redis bazu: {ex.Message}");
                throw;
            }
        }

        private async Task UpdateActivityInDatabaseAsync(Activity activity)
        {
            try
            {
                await _redisDatabase.SetAsync($"Activity_{activity.ActivityId}", activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja aktivnosti u Redis bazi: {ex.Message}");
                throw;
            }
        }

        private async Task DeleteActivityFromDatabaseAsync(int activityId)
        {
            try
            {
                await _redisDatabase.RemoveAsync($"Activity_{activityId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja aktivnosti iz Redis baze sa ID {activityId}: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetFromCacheAsync(string key)
        {
            try
            {
                return await _cache.GetStringAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobijanja iz keša: {ex.Message}");
                throw;
            }
        }
    }
}
