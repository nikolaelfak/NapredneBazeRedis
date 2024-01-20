using FitAndFun.Models;
using FitAndFun.Common;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis; 
using Microsoft.AspNetCore.Mvc;

namespace FitAndFun.Services
{
    public class UserService : IUserService
    {
        private readonly IDistributedCache _cache;
        private readonly IRedisDatabase _redisDatabase;
        //private readonly ICurrentUserService _currentUserService;

        public UserService(IDistributedCache cache, IRedisDatabase redisDatabase)
        {
            _cache = cache;
            _redisDatabase = redisDatabase;
        }

        public async Task<IEnumerable<string>> GetUsersSubscribedToChannel(string channel)
        {
            var key = $"SubscribedUsers_{channel}";

            var subscribedUsers = await _redisDatabase.GetAsync<IEnumerable<string>>(key);
            return subscribedUsers ?? Enumerable.Empty<string>();
        }

        public async Task SubscribeUserToChannel(string userId, string channel)
        {
            var key = $"SubscribedUsers_{channel}";

            // Dobijamo postojeće pretplaćene korisnike
            var subscribedUsers = await _redisDatabase.GetAsync<List<string>>(key) ?? new List<string>();

            // Dodajemo korisnika u listu pretplaćenih
            if (!subscribedUsers.Contains(userId))
            {
                subscribedUsers.Add(userId);
            }

            // Čuvamo ažuriranu listu pretplaćenih korisnika
            await _redisDatabase.SetAsync(key, subscribedUsers);
        }

public async Task<User> GetUserById(int userId)
{
    try
    {
        var userFromCache = await GetFromCacheAsync($"User_{userId}");

        if (userFromCache != null)
        {
            Console.WriteLine($"Deserialized user from cache: {userFromCache}");
            return JsonSerializer.Deserialize<User>(userFromCache);
        }

        // Ako nije pronađeno u kešu, uzmi iz Redis baze i dodaj u keš
        var userFromDatabase = await GetUserFromDatabaseAsync(userId);

        if (userFromDatabase != null)
        {
            Console.WriteLine($"Serialized user for cache: {JsonSerializer.Serialize(userFromDatabase)}");
            await AddToCacheAsync($"User_{userId}", JsonSerializer.Serialize(userFromDatabase));
        }

        return userFromDatabase;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Greška prilikom dobavljanja korisnika sa ID {userId}: {ex.Message}");
        throw;
    }
}


      public async Task<IActionResult> GetLoggedInUser([FromQuery] string username)
    {
        try
        {
            var user = await GetUserByUsername(username);

            if (user != null)
            {
                return new OkObjectResult(user);
            }
            else
            {
                return new NotFoundObjectResult("Korisnik nije pronađen.");
            }
        }
        catch (Exception ex)
        {
            return new ObjectResult($"Internal Server Error: {ex.Message}")
            {
                StatusCode = 500
            };
        }
    }

public async Task<User> ValidateUser(string username, string password)
    {
        var cachedUser = await GetFromCacheAsync($"User_{username}");

        if (!string.IsNullOrEmpty(cachedUser))
        {
            var cachedUserObject = JsonSerializer.Deserialize<User>(cachedUser);
            return cachedUserObject.Password == password ? cachedUserObject : null;
        }

        var user = await GetUserFromDatabaseAsync2(username, password);

        if (user != null)
        {
            await AddToCacheAsync($"User_{username}", JsonSerializer.Serialize(user));
        }

        return user;
    }


private async Task<User> GetUserFromDatabaseAsync3(string username)
{
    try
    {
        // Pretraži bazu podataka direktno po username-u
        var usersFromDatabase = await GetUsersFromDatabaseAsync();
        return usersFromDatabase.FirstOrDefault(u => u.Username == username);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting user from Redis database for username {username}: {ex.Message}");
        throw;
    }
}

    public async Task<User> GetUserByUsername(string username)
    {
        var cachedUser = await GetFromCacheAsync($"User_Username_{username}");

        if (cachedUser != null)
        {
            try
            {
                var user = JsonSerializer.Deserialize<User>(cachedUser);
                if (user != null)
                {
                    Console.WriteLine($"Deserialized user from cache: {JsonSerializer.Serialize(user)}");
                    return user;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing user from cache: {ex.Message}");
            }
        }

        // Implementacija logike za dobavljanje korisnika iz baze ako nije u kešu
        var userFromDatabase = await GetUserFromDatabaseAsync(username);

        if (userFromDatabase != null)
        {
            Console.WriteLine($"User found in database for username: {JsonSerializer.Serialize(userFromDatabase)}");

            // Dodajte korisnika u keš
            await AddToCacheAsync($"User_Username_{username}", userFromDatabase);

            return userFromDatabase;
        }
        else
        {
            Console.WriteLine($"User not found in database for username: {username}");
            return null;
        }
    }


//  public async Task<IEnumerable<User>> GetAllUsers()
// {
//     var usersFromCache = await GetFromCacheAsync("Users");

//     if (usersFromCache != null)
//     {
//         try
//         {
//             var cachedUsers = JsonSerializer.Deserialize<IEnumerable<User>>(usersFromCache);
//             if (cachedUsers != null)
//             {
//                 Console.WriteLine($"Deserialized users from cache: {JsonSerializer.Serialize(cachedUsers)}");
//                 return cachedUsers;
//             }
//         }
//         catch (JsonException ex)
//         {
//             Console.WriteLine($"Error deserializing users from cache: {ex.Message}");
//         }
//     }

//     // Implementacija logike za dobavljanje korisnika iz baze ako nisu u kešu
// var usersFromDatabase = await GetUsersFromDatabaseAsync();

// if (usersFromDatabase != null && usersFromDatabase.Any())
// {
//     Console.WriteLine($"Users found in database: {JsonSerializer.Serialize(usersFromDatabase)}");

//     // Dodajte korisnike u keš
//     await AddToCacheAsync("Users", usersFromDatabase);

//     return usersFromDatabase;
// }
// else
// {
//     Console.WriteLine($"No users found in database.");
//     return Enumerable.Empty<User>();
// }
// }

private async Task<IEnumerable<User>> GetUsersFromDatabaseAsync()
{
    try
    {
        // Pretpostavljamo da imate metodu u RedisDatabase klasi koja vraća sve korisnike.
        var usersFromDatabase = await _redisDatabase.GetAsync<IEnumerable<User>>("AllUsers");

        return usersFromDatabase ?? Enumerable.Empty<User>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting users from Redis database: {ex.Message}");
        throw;
    }
}

public async Task<IEnumerable<User>> GetAllUsers()
{
    try
    {
        var redisKey = "AllUsers";
        var cachedUsers = await _redisDatabase.GetAsync<IEnumerable<User>>(redisKey);

        if (cachedUsers == null)
        {
            var usersFromDatabase = await GetAllUsersFromDatabaseAsync();

            if (usersFromDatabase != null)
            {
                await AddToCacheAsync(redisKey, usersFromDatabase);
            }

            return usersFromDatabase;
        }

        return cachedUsers;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Greška prilikom dobijanja svih korisnika iz Redis baze: {ex.Message}");
        throw;
    }
}

private async Task<IEnumerable<User>> GetAllUsersFromDatabaseAsync()
{
    try
    {
        var userKeys = await _redisDatabase.GetAllKeysAsync("User_*");

        var users = new List<User>();
        foreach (var key in userKeys)
        {
            var user = await _redisDatabase.GetAsync<User>(key);
            if (user != null)
            {
                users.Add(user);
            }
        }

        return users;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Greška prilikom dobijanja svih korisnika iz baze podataka: {ex.Message}");
        throw;
    }
}



        public async Task<string> GetCachedUserById(int userId)
        {
            try
            {
                var userFromCache = await GetFromCacheAsync($"User_{userId}");

                if (userFromCache == null)
                {
                    return null;
                }

                return userFromCache;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja korisnika sa ID {userId} iz keša: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCachedAllUsers()
        {
            try
            {
                var usersFromCache = await GetFromCacheAsync("AllUsers");

                if (usersFromCache == null)
                {
                    return Enumerable.Empty<string>();
                }

                return JsonSerializer.Deserialize<IEnumerable<string>>(usersFromCache);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja svih korisnika iz keša: {ex.Message}");
                throw;
            }
        }

        public async Task AddUser(User user)
        {
            try
            {
                // Dodaj u Redis bazu
                await AddUserToDatabaseAsync(user);

                // Očisti keš
                await InvalidateCacheAsync($"User_{user.UserId}");
                await InvalidateCacheAsync("AllUsers");

                // Postavljamo trenutnog korisnika
                //_currentUserService.SetCurrentUserId(user.UserId);
                //_currentUserService.SetCurrentUserId(user.UserId.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja korisnika: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                // Ažuriraj u Redis bazi
                await UpdateUserInDatabaseAsync(user);

                // Očisti keš
                await InvalidateCacheAsync($"User_{user.UserId}");
                await InvalidateCacheAsync("AllUsers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja korisnika: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteUser(int userId)
        {
            try
            {
                // Obriši iz Redis baze
                await DeleteUserFromDatabaseAsync(userId);

                // Očisti keš
                await InvalidateCacheAsync($"User_{userId}");
                await InvalidateCacheAsync("AllUsers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja korisnika sa ID {userId}: {ex.Message}");
                throw;
            }
        }

        // public async Task<IEnumerable<User>> GetAllUsersFromDatabaseAsync()
        // {
        //     try
        //     {
        //         return await _redisDatabase.GetAsync<IEnumerable<User>>("AllUsers");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Greška prilikom dobavljanja svih korisnika iz Redis baze: {ex.Message}");
        //         throw;
        //     }
        // }

        private async Task<User> GetUserFromDatabaseAsync(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<User>($"User_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja korisnika iz Redis baze po ID {userId}: {ex.Message}");
                throw;
            }
        }

        private async Task<User> GetUserFromDatabaseAsync(string username)
{
    try
    {
        var userFromDatabase = await _redisDatabase.GetAsync<User>($"User_Username_{username}");

        if (userFromDatabase != null)
        {
            Console.WriteLine($"User found in database: {JsonSerializer.Serialize(userFromDatabase)}");
            return userFromDatabase;
        }
        else
        {
            Console.WriteLine($"User not found in database for username: {username}");
            return null;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting user from Redis database for username {username}: {ex.Message}");
        throw;
    }
}

private async Task<User> GetUserFromDatabaseAsync2(string username, string password)
    {
        try
        {
            var usersFromDatabase = await GetUsersFromDatabaseAsync();
            var user = usersFromDatabase.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Console.WriteLine($"User found in database: {JsonSerializer.Serialize(user)}");
                return user;
            }
            else
            {
                Console.WriteLine($"User not found in database for username: {username}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user from Redis database for username {username}: {ex.Message}");
            throw;
        }
    }



        private async Task AddUserToDatabaseAsync(User user)
        {
            try
            {
                await _redisDatabase.SetAsync($"User_{user.UserId}", user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja korisnika u Redis bazu: {ex.Message}");
                throw;
            }
        }

        private async Task UpdateUserInDatabaseAsync(User user)
        {
            try
            {
                await _redisDatabase.SetAsync($"User_{user.UserId}", user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja korisnika u Redis bazi: {ex.Message}");
                throw;
            }
        }

        private async Task DeleteUserFromDatabaseAsync(int userId)
        {
            try
            {
                await _redisDatabase.RemoveAsync($"User_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja korisnika iz Redis baze sa ID {userId}: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetFromCacheAsync(string key)
        {
            try
            {
                // Uzmi iz keša
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


        private async Task RemoveFromCacheAsync(string key)
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

        public async Task InvalidateCacheAsync(string key)
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