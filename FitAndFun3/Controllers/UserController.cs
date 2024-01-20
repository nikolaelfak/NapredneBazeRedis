using FitAndFun.Services;
using Microsoft.AspNetCore.Mvc;
using FitAndFun.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FitAndFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly ICurrentUserService _currentUserService;

        public UserController(IUserService userService/*, ICurrentUserService currentUserService*/)
        {
            _userService = userService;
            //_currentUserService = currentUserService;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(User user)
        {
            try
            {
                _userService.AddUser(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

[HttpPut("UpdateUser/{id}")]
public async Task<IActionResult> UpdateUser(int id, User updatedUser)
{
    try
    {
        var existingUser = await _userService.GetUserById(id);

        if (existingUser == null)
        {
            return NotFound();
        }

        // Ažuriraj korisnika
        existingUser.Username = updatedUser.Username;
        existingUser.Password = updatedUser.Password;
        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Email = updatedUser.Email;

        await _userService.UpdateUser(existingUser);

        return Ok(existingUser);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
    }
}


        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userService.DeleteUser(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // [HttpPost("SubscribeToUser/{targetUserId}")]
        // public async Task<IActionResult> SubscribeToUser(string targetUserId)
        // {
        //     try
        //     {
        //         // Pretpostavljamo da imate trenutnog korisnika iz nekog mehanizma autentifikacije
        //         var currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //         if (string.IsNullOrEmpty(currentUserId))
        //         {
        //             return Unauthorized("Nedostaju informacije o trenutnom korisniku.");
        //         }

        //         await _userService.SubscribeUserToChannel(currentUserId, targetUserId);
        //         return Ok($"Uspešno ste se pretplatili na korisnika sa ID: {targetUserId}");
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest($"Greška prilikom pretplate na korisnika: {ex.Message}");
        //     }
        // }

        //STARO PRETPLACIVANJE (NE RADI)
        // [HttpPost("SubscribeToUser/{currentUserId}/{targetUserId}")]
        // public async Task<IActionResult> SubscribeToUser(string currentUserId, string targetUserId)
        // {
        //     try
        //     {
        //         Console.WriteLine($"CurrentUserID: {currentUserId}, TargetUserID: {targetUserId}");
        //         // Proverite da li je trenutni korisnik isti kao korisnik u zahtevu
        //         // if (currentUserId != targetUserId)
        //         // {
        //         //     return Unauthorized("Neovlašćen pristup.");
        //         // }

        //         await _userService.SubscribeUserToChannel(currentUserId, targetUserId);
        //         return Ok($"Uspešno ste se pretplatili na korisnika sa ID: {targetUserId}");
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest($"Greška prilikom pretplate na korisnika: {ex.Message}");
        //     }
        // }

//         [HttpPost("SubscribeToUser/{targetUserId}")]
//         [Authorize]
//         public async Task<IActionResult> SubscribeToUser(string targetUserId)
//         {
//             try
//             {
//                 var currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//                 // Provera da li je trenutni korisnik isti kao korisnik u zahtevu
//                 if (currentUserId != targetUserId)
//                 {
//                     await _userService.SubscribeUserToChannel(currentUserId, targetUserId);
//                     return Ok($"Uspešno ste se pretplatili na korisnika sa ID: {targetUserId}");
//                 }

//                 return Unauthorized("Neovlašćen pristup.");
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest($"Greška prilikom pretplate na korisnika: {ex.Message}");
//             }
//         }


//         [HttpGet("GetSubscriptions")]
//         public async Task<IActionResult> GetSubscriptions()
//         {
//             try
//             {
//                 // Pretpostavljamo da imate trenutnog korisnika iz nekog mehanizma autentifikacije
//                 var currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//                 if (string.IsNullOrEmpty(currentUserId))
//                 {
//                     return Unauthorized("Nedostaju informacije o trenutnom korisniku.");
//                 }

//                 var subscribedUsers = await _userService.GetUsersSubscribedToChannel(currentUserId);
//                 return Ok(subscribedUsers);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest($"Greška prilikom dobijanja pretplata: {ex.Message}");
//             }
//         }

//         [HttpPost("Register")]
// public IActionResult Register(User user)
// {
//     try
//     {
//         // Provera da li korisnik već postoji
//         var existingUser = _userService.GetUserByUsername(user.Username);
//         if (existingUser == null)
//         {
//             return BadRequest("Korisnik već postoji.");
//         }

//         // Postavljanje dodatnih informacija za registraciju
//         user.RegistrationDate = DateTime.Now;
//         //user.IsActive = true;

//         // Dodavanje korisnika
//         _userService.AddUser(user);

//         // Vraćanje uspešnog odgovora
//         return Ok("Registracija uspešna!");
//     }
//     catch (Exception ex)
//     {
//         // Vraćanje greške u slučaju problema
//         return StatusCode(500, $"Internal Server Error: {ex.Message}");
//     }
// }

//         [HttpPost("Login")]
// public IActionResult Login(User loginRequest)
// {
//     try
//     {
//         // Provera korisničkog imena i lozinke (možete dodati dodatnu logiku prema potrebi)
//         var existingUser = _userService.ValidateUser(loginRequest.Username, loginRequest.Password);

//         if (existingUser != null)
//         {
//             // Uspela prijava - možete generisati token, postaviti sesiju, ili slično
//             // Trenutno, samo vraćamo potvrdu uspešne prijave
//             return Ok("Uspešna prijava!");
//         }

//         // Neuspešna prijava
//         return BadRequest("Pogrešno korisničko ime ili lozinka.");
//     }
//     catch (Exception ex)
//     {
//         return StatusCode(500, $"Internal Server Error: {ex.Message}");
//     }
// }

[HttpGet("GetUserByUsername")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsername(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet]
public async Task<IActionResult> GetAllUsers()
{
    try
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
    }
}

    
    [HttpGet("GetLoggedInUser")]
    public async Task<IActionResult> GetLoggedInUser([FromQuery] string username)
    {
        try
        {
            var user = await _userService.GetUserByUsername(username);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("Korisnik nije pronađen.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    }
}
