using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitAndFun.Common;
using FitAndFun.Services;

public class RedisSubscriberService : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IRedisDatabase _redisDatabase;
    private readonly IUserService _userService;

    public RedisSubscriberService(IConnectionMultiplexer redis, IRedisDatabase redisDatabase, IUserService userService)
    {
        _redis = redis;
        _redisDatabase = redisDatabase;
        _userService = userService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();

        await subscriber.SubscribeAsync("novi_cilj_channel", async (channel, message) =>
        {
            Console.WriteLine($"Novi cilj dodat: {message}");

            // Dobijamo listu korisnika koji su pretplaćeni na ovaj kanal
            var subscribedUsers = await _userService.GetUsersSubscribedToChannel(channel);

            // Ako želimo, možemo dalje obrađivati primljenu poruku samo za korisnike koji su pretplaćeni
            foreach (var userId in subscribedUsers)
            {
                Console.WriteLine($"Poslato obaveštenje korisniku {userId}: {message}");
            }
        });

        await PretplatiKorisnikeNaKanal();
    }

    private async Task PretplatiKorisnikeNaKanal()
    {
        var usersToSubscribe = await _userService.GetUsersSubscribedToChannel("novi_cilj_channel");

        Console.WriteLine("Pretplaćeni korisnici:");

        foreach (var userId in usersToSubscribe)
        {
            await _userService.SubscribeUserToChannel(userId, "novi_cilj_channel");
            Console.WriteLine($" - {userId}");
        }
    }

}
