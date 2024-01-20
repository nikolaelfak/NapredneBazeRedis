// using Microsoft.AspNetCore.Identity;
// using Newtonsoft.Json;
// using StackExchange.Redis;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;


// public class YourRedisRoleStore : IRoleStore<IdentityRole>
// {
//     private readonly IConnectionMultiplexer _redis;

//     public YourRedisRoleStore(IConnectionMultiplexer redis)
//     {
//         _redis = redis ?? throw new ArgumentNullException(nameof(redis));
//     }

//     public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.StringSetAsync($"role:{role.Id}", JsonConvert.SerializeObject(role));

//         return IdentityResult.Success;
//     }

//     public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.StringSetAsync($"role:{role.Id}", JsonConvert.SerializeObject(role));

//         return IdentityResult.Success;
//     }

//     public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         await db.KeyDeleteAsync($"role:{role.Id}");

//         return IdentityResult.Success;
//     }

//     public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         var roleJson = db.StringGet($"role:{roleId}");
//         return Task.FromResult(roleJson.HasValue ? JsonConvert.DeserializeObject<IdentityRole>(roleJson) : null);
//     }

//     public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
//     {
//         var db = _redis.GetDatabase();
//         var roleId = db.StringGet($"role:name:{normalizedRoleName}");
//         return FindByIdAsync(roleId, cancellationToken);
//     }

//     public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
//     {
//         return Task.FromResult(role.Id);
//     }

//     public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
//     {
//         return Task.FromResult(role.Name);
//     }

//     public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
//     {
//         role.Name = roleName;
//         return Task.CompletedTask;
//     }

//     public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
//     {
//         return Task.FromResult(role.NormalizedName);
//     }

//     public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
//     {
//         role.NormalizedName = normalizedName;
//         return Task.CompletedTask;
//     }

//     public void Dispose()
//     {
//         // Implementacija za oslobađanje resursa ako je potrebno
//     }
// }