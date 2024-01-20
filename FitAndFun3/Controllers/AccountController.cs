// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using FitAndFun.Common;
// using FitAndFun.Models; 
// using FitAndFun.Services;

// [Route("[controller]")]
// public class AccountController : Controller
// {
//     private readonly IUserService _userService;

//     public AccountController(IUserService userService)
//     {
//         _userService = userService;
//     }

//     [HttpGet("Login")]
//     [AllowAnonymous]
//     public IActionResult Login()
//     {
//         // Prikazivanje forme za prijavu
//         return View();
//     }

//     [HttpPost("Login")]
//     [AllowAnonymous]
//     public async Task<IActionResult> Login(LoginViewModel model)
//     {
//         // Provera korisničkih kredencijala
//         var user = await _userService.ValidateUserAsync(model.Username, model.Password);

//         if (user != null)
//         {
//             // Ako su kredencijali ispravni, postavite trenutnog korisnika
//             var claims = new List<Claim>
//             {
//                 new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Postavljanje ID-ja korisnika
//                 // Dodajte dodatne informacije o korisniku ako je potrebno
//             };

//             var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//             var authProperties = new AuthenticationProperties
//             {
//                 // Postavite svojstva autentikacije ako je potrebno
//             };

//             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

//             return RedirectToAction("Index", "Home"); // Preusmeravanje na odgovarajuću stranicu
//         }

//         // Ako su kredencijali netačni, vratite korisnika na stranicu za prijavu sa porukom
//         ModelState.AddModelError(string.Empty, "Pogrešno korisničko ime ili lozinka");
//         return View(model);
//     }

//     [HttpGet("Logout")]
//     public async Task<IActionResult> Logout()
//     {
//         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//         return RedirectToAction("Index", "Home"); // Preusmeravanje na odgovarajuću stranicu
//     }
// }
