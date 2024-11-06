using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly PackageService _packageService;
        public PackageController(PackageService packageService)
        {
           _packageService = packageService;
        }

        [HttpGet("available-packages")]
        public async Task<IActionResult> GetAvailablePackages([FromQuery] string userid)
        {
            try
            {
                var packages = await _packageService.GetAvailablePackagesAsync(userid);
                if (packages == null || packages.Count() == 0)
                {
                    return NotFound("There is no available package.");
                }
                else
                {
                    return Ok(packages);
                }
                
            }
            catch (Exception ex)
            {
                return NotFound("There is no available package.");
            }
            
        }

        [HttpPost("buy-packages")]
        public async Task<IActionResult> BuyPackage(BuyRequest buyRequest)
        {
            try
            {
                bool result = await _packageService.BuyPackage(buyRequest.userid, buyRequest.packageId);

                if (result)
                    return Ok("Package buy successfully.");

                return BadRequest("Package buy fail.");

            }
            catch (Exception ex)
            {
                return NotFound("Package buy fail.");
            }

        }


        [HttpGet("purchased-packages")]
        public async Task<IActionResult> GetUserPackages(string userId)
        {
            try
            {
                var userPackages = await _packageService.GetUserPackagesAsync(userId);
                if(userPackages == null || userPackages.Count() == 0)
                {
                    return NotFound("There is no available package.");
                }
                return Ok(userPackages.Select(up => new
                {
                    up.Package.Name,
                    up.Package.Country,
                    up.Package.Price,
                    up.Package.Credits,
                    up.PurchaseDate,
                    up.ExpirationDate,
                    Status = up.IsExpired ? "Expired" : "Active",  // Status based on expiration
                    up.RemainingCredits
                }));
            }
            catch (Exception ex)
            {
                return NotFound("There is no available package.");
            }
          
        }

    }
}
