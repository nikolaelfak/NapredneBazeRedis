// using Microsoft.AspNetCore.Identity;
// using Newtonsoft.Json;
// using StackExchange.Redis;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;

// public class YourRedisUserStore : IUserStore<IdentityUser>,
//                                    IUserRoleStore<IdentityUser>
// {
//     private readonly IConnectionMultiplexer _redis;

//     public YourRedisUserStore(IConnectionMultiplexer redis)
//     {
//         _redis = redis ?? throw new ArgumentNullException(nameof(redis));
//     }

//     public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.StringSetAsync($"user:{user.Id}", JsonConvert.SerializeObject(user));

//         return IdentityResult.Success;
//     }

//     public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.StringSetAsync($"user:{user.Id}", JsonConvert.SerializeObject(user));

//         return IdentityResult.Success;
//     }

//     public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.KeyDeleteAsync($"user:{user.Id}");

//         return IdentityResult.Success;
//     }

//     public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         var userJson = db.StringGet($"user:{userId}");
//         return Task.FromResult(userJson.HasValue ? JsonConvert.DeserializeObject<IdentityUser>(userJson) : null);
//     }

//     public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         var userId = db.StringGet($"user:name:{normalizedUserName}");
//         return FindByIdAsync(userId, cancellationToken);
//     }

//     // Implementacija IUserRoleStore metoda
//     public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.SetAddAsync($"user:roles:{roleName}", user.Id);
//     }

//     public async Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.SetRemoveAsync($"user:roles:{roleName}", user.Id);
//     }

//     public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         var roles = await db.SetMembersAsync($"user:roles:{user.Id}");
//         return roles.Select(role => role.ToString()).ToList();
//     }

//     public Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         return db.SetContainsAsync($"user:roles:{user.Id}", roleName);
//     }

//     public void Dispose()
//     {
//         // Implementacija za oslobađanje resursa ako je potrebno
//     }
// }
