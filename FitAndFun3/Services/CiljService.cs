using FitAndFun.Models;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FitAndFun.Common;
using StackExchange.Redis;
//using Newtonsoft.Json;

namespace FitAndFun.Services
{
    public class CiljService : ICiljService
    {
        private readonly IDistributedCache _cache;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IConnectionMultiplexer _redisConnection;

        public CiljService(IDistributedCache cache, IRedisDatabase redisDatabase, IConnectionMultiplexer redisConnection)
        {
            _cache = cache;
            _redisDatabase = redisDatabase;
            _redisConnection = redisConnection;
        }

        public async Task<Cilj> GetCiljById(int ciljId)
{
    var cacheKey = $"Cilj_{ciljId}";

    var cachedCilj = await GetFromCacheAsync(cacheKey);

    if (cachedCilj != null)
    {
        try
        {
            var cilj = JsonSerializer.Deserialize<Cilj>(cachedCilj);
            if (cilj != null)
            {
                Console.WriteLine($"Deserialized cilj from cache: {JsonSerializer.Serialize(cilj)}");
                return cilj;
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing cilj from cache: {ex.Message}");
        }
    }

    // Implementacija logike za dobavljanje cilja iz baze ako nije u kešu
    var ciljFromDatabase = await GetCiljFromDatabaseAsync(ciljId);

    // Dodajte cilj u keš
    await AddToCacheAsync(cacheKey, ciljFromDatabase);

    return ciljFromDatabase;
}


//OVA FUNKCIJA JE NAPRAVLJENA DA KORISTI Newtonsoft
//                 public async Task<Cilj> GetCiljById(int ciljId)
// {
//     var cacheKey = $"Cilj_{ciljId}";

//     try
//     {
//         var cachedCilj = await GetFromCacheAsync(cacheKey);

//         if (cachedCilj == null)
//         {
//             // Ako nije pronađeno u kešu, uzmi iz Redis baze i dodaj u keš
//             var ciljFromDatabase = await GetCiljFromDatabaseAsync(ciljId);

//             if (ciljFromDatabase != null)
//             {
//                 Console.WriteLine($"Cilj pre dodavanja u keš: {JsonConvert.SerializeObject(ciljFromDatabase)}");
//                 await AddToCacheAsync(cacheKey, JsonConvert.SerializeObject(ciljFromDatabase));
//             }

//             return ciljFromDatabase;
//         }

//         Console.WriteLine($"Deserialized cilj from cache: {JsonConvert.SerializeObject(cachedCilj)}");
//         return JsonConvert.DeserializeObject<Cilj>(cachedCilj);
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Greška prilikom dobavljanja cilja po ID iz keša: {ex.Message}");
//         throw;
//     }
// }
        
        public async Task<IEnumerable<Cilj>> GetCiljeviByUserId(int userId)
        {
            try
            {
                var cachedCiljevi = await _redisDatabase.GetAsync<IEnumerable<Cilj>>($"Ciljevi_{userId}");

                if (cachedCiljevi != null)
                {
                    Console.WriteLine($"Deserialized ciljevi from cache for UserId {userId}: {JsonSerializer.Serialize(cachedCiljevi)}");
                    return cachedCiljevi;
                }

                // Ako nije pronađeno u kešu, uzmi iz Redis baze i dodaj u keš
                var ciljFromDatabase = await GetCiljeviFromDatabaseAsync(userId);

                if (ciljFromDatabase != null)
                {
                    var ciljeviList = new List<Cilj> { ciljFromDatabase }; // Kreirajte listu sa jednim ciljem
                    Console.WriteLine($"Deserialized cilj from database for UserId {userId}: {JsonSerializer.Serialize(ciljFromDatabase)}");
                    await _redisDatabase.SetAsync($"Ciljevi_{userId}", ciljeviList);
                    return ciljeviList; // Vratite listu sa jednim ciljem
                }

                return Enumerable.Empty<Cilj>(); // Vratite praznu listu ako nema podataka
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja ciljeva iz Redis baze po ID korisnika: {ex.Message}");
                throw;
            }
        }

        private async Task<Cilj> GetCiljeviFromDatabaseAsync(int ciljId)
        {
            try
            {
                var ciljeviFromDatabase = await _redisDatabase.GetAsync<Cilj>($"Cilj_{ciljId}");

                return ciljeviFromDatabase;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja ciljeva iz baze po ID korisnika: {ex.Message}");
                throw;
            }
        }


        public async Task AddCilj(Cilj cilj)
        {
            try
            {
                await _redisDatabase.SetAsync($"Cilj_{cilj.CiljId}", cilj);
                await InvalidateCacheAsync($"Ciljevi_{cilj.UserId}");

                Console.WriteLine($"Šaljem poruku na kanal 'novi_cilj_channel': {JsonSerializer.Serialize(cilj)}");
                // Slanje obaveštenja o novom cilju
                var redisSubscriber = _redisConnection.GetSubscriber();
                await redisSubscriber.PublishAsync("novi_cilj_channel", JsonSerializer.Serialize(cilj));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja cilja: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateCilj(Cilj cilj)
        {
            try
            {
                await _redisDatabase.SetAsync($"Cilj_{cilj.CiljId}", cilj);
                await InvalidateCacheAsync($"Ciljevi_{cilj.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja cilja: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteCilj(int ciljId)
        {
            try
            {
                await _redisDatabase.RemoveAsync($"Cilj_{ciljId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja cilja sa ID {ciljId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Cilj>> GetCachedCiljeviByUserId(int userId)
        {
            var cacheKey = $"Ciljevi_{userId}";

            try
            {
                var cachedCiljevi = await GetFromCacheAsync(cacheKey);

                if (cachedCiljevi == null)
                {
                    // Ako nije pronađeno u kešu, uzmi iz Redis baze i dodaj u keš
                    var ciljeviFromDatabase = await GetCiljeviFromDatabaseByUserIdAsync(userId);

                    if (ciljeviFromDatabase != null)
                    {
                        await AddToCacheAsync(cacheKey, ciljeviFromDatabase);
                    }

                    return ciljeviFromDatabase;
                }

                return JsonSerializer.Deserialize<IEnumerable<Cilj>>(cachedCiljevi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja ciljeva iz keša: {ex.Message}");
                throw;
            }
        }

        private async Task<Cilj> GetCiljFromDatabaseAsync(int ciljId)
        {
            try
            {
                return await _redisDatabase.GetAsync<Cilj>($"Cilj_{ciljId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja cilja iz Redis baze po ID: {ex.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Cilj>> GetCiljeviFromDatabaseByUserIdAsync(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<IEnumerable<Cilj>>($"Ciljevi_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja ciljeva iz Redis baze po ID korisnika: {ex.Message}");
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
                Console.WriteLine($"Greška prilikom dobavljanja iz keša: {ex.Message}");
                throw;
            }
        }

        private async Task AddToCacheAsync(string key, object value)
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
    }
}
