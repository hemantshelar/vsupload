using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<MyUser> userManager = null;
		//private readonly SignInManager<MyUser> signInManager = null;

		public HomeController(UserManager<MyUser> userManager)
		{
			this.userManager = userManager;
			
		}
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel registerModel)
		{
			var identityResult = await this.userManager.CreateAsync(new MyUser()
			{
				Id = Guid.NewGuid().ToString(),
				UserName = registerModel.UserName,				
				PasswordHash = registerModel.Password
			},registerModel.Password);

			if (identityResult.Succeeded)
				return View();
			else
			{
				int id = 1;
				foreach (var item in identityResult.Errors)
				{
					ModelState.AddModelError(id.ToString(), item.Description);
					id++;
				}
				return View();
			}
		}

		public IActionResult Login()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> Login(LoginModel loginModel)
		{
			var user = await this.userManager.FindByNameAsync(loginModel.UserName);
			if (  user != null  )
			{
				bool bValue = await this.userManager.CheckPasswordAsync(user, loginModel.Password);
				if ( bValue == true )
				{
					ClaimsIdentity claimsIdentity = new ClaimsIdentity("cookies");
					claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
					claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
					await HttpContext.SignInAsync("cookies",new ClaimsPrincipal(claimsIdentity));
					return RedirectToAction("About");
				}
			}
			this.ModelState.AddModelError("", "Invalid username or password.");
			return View();
		}
	}
}
