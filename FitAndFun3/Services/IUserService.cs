using System.Collections.Generic;
using System.Threading.Tasks;
using FitAndFun.Models;

namespace FitAndFun.Services
{
    public interface IUserService
    {
        Task<User> GetUserById(int userId);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int userId);
        Task<IEnumerable<User>> GetAllUsers();
        Task<string> GetCachedUserById(int userId);
        Task<IEnumerable<string>> GetCachedAllUsers();
        Task InvalidateCacheAsync(string key);
        Task<IEnumerable<string>> GetUsersSubscribedToChannel(string channel);
        Task SubscribeUserToChannel(string userId, string channel);
        Task<User> GetUserByUsername(string username);
        //Task<User> ValidateUser(string username, string password);
        //Task<User> ValidateUserAsync(string username, string password);
    }
}
