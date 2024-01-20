using FitAndFun.Models;
using FitAndFun.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using FitAndFun.Common;


namespace FitAndFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        //private readonly IRedisCacheService _redisCacheService; // Dodajte Redis keš servis
        private readonly IRedisDatabase _redisDatabase;
private readonly IRedisCacheService _redisCacheService;
//private readonly IUserService _userService;

public ActivityController(IActivityService activityService, IRedisDatabase redisDatabase, IRedisCacheService redisCacheService)
{
    _activityService = activityService;
    _redisDatabase = redisDatabase;
    _redisCacheService = redisCacheService;
    //_userService = userService;
}

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetActivity(int id)
        // {
        //     try
        //     {
        //         var activity = await _activityService.GetActivityById(id);

        //         if (activity == null)
        //         {
        //             return NotFound();
        //         }

        //         return Ok(activity);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Internal Server Error: {ex.Message}");
        //     }
        // }

        [HttpGet("{id}")]
public async Task<IActionResult> GetActivity(int id)
{
    try
    {
        var cacheKey = $"Activity_{id}";
        Activity activity;

        // Pokušaj dohvatiti iz keša
        var cachedData = await _redisCacheService.GetAsync<string>(cacheKey);

        if (cachedData != null)
        {
            // Ako je u kešu, vrati iz keša
            activity = JsonConvert.DeserializeObject<Activity>(cachedData);

            // Postavi zaglavlje kako bi klijent znao da je podatak došao iz keša
            Response.Headers.Add("X-Data-From-Cache", "true");
        }
        else
        {
            // Ako nije u kešu, dohvati iz baze
            activity = await _activityService.GetActivityById(id);

            if (activity == null)
            {
                return NotFound();
            }

            // Serijalizuj i dodaj u keš
            var serializedData = JsonConvert.SerializeObject(activity);
            await _redisCacheService.SetAsync(cacheKey, serializedData);

            // Postavi zaglavlje kako bi klijent znao da je podatak došao iz baze
            Response.Headers.Add("X-Data-From-Cache", "false");
        }

        return Ok(activity);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
    }
}


        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity)
        {
            try
            {
                await _activityService.AddActivity(activity);

                return CreatedAtAction(nameof(GetActivity), new { id = activity.ActivityId }, activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, Activity updatedActivity)
        {
            try
            {
                var existingActivity = await _activityService.GetActivityById(id);

                if (existingActivity == null)
                {
                    return NotFound();
                }

                existingActivity.Name = updatedActivity.Name;
                existingActivity.UserId = updatedActivity.UserId;
                existingActivity.Duration = updatedActivity.Duration;
                existingActivity.Date = updatedActivity.Date;
                existingActivity.Location = updatedActivity.Location;
                existingActivity.ActivityType = updatedActivity.ActivityType;
                existingActivity.AdditionalDescription = updatedActivity.AdditionalDescription;

                await _activityService.UpdateActivity(existingActivity);

                return Ok(existingActivity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            try
            {
                var activity = await _activityService.GetActivityById(id);

                if (activity == null)
                {
                    return NotFound();
                }

                await _activityService.DeleteActivity(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetActivitiesByUser(int userId)
        {
            try
            {
                var activities = await _activityService.GetActivitiesByUserId(userId);

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            try
            {
                var activities = await _activityService.GetAllActivities();
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

// [HttpPost("{activityId}/comments")]
// public async Task<IActionResult> AddComment(int activityId, [FromBody] Comment comment)
// {
//     try
//     {
//         // Validacija unosa
//         if (string.IsNullOrWhiteSpace(comment.Text))
//         {
//             return BadRequest("Tekst komentara ne sme biti prazan.");
//         }

//         // Provera postojanja aktivnosti
//         var activity = await _activityService.GetActivityById(activityId);
//         if (activity == null)
//         {
//             return NotFound("Aktivnost nije pronađena.");
//         }

//         // Ako nema liste komentara, inicijalizujte je
//         if (activity.Comments == null)
//         {
//             activity.Comments = new List<Comment>();
//         }

//         // Dodavanje komentara u aktivnost
//         comment.ActivityId = activityId;
//         comment.UserId = _userService.GetUserById(); // Koristite funkciju koja dobavlja trenutnog korisnika

//         activity.Comments.Add(comment);

//         // Ažuriranje aktivnosti u servisu
//         await _activityService.UpdateActivity(activity);

//         return Ok(comment);
//     }
//     catch (Exception ex)
//     {
//         // Logovanje greške
//         Console.WriteLine($"Greška prilikom dodavanja komentara: {ex.Message}");
//         return StatusCode(500, "Internal Server Error");
//     }
// }
    }
}
