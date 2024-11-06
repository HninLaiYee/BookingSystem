using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace BookingSystem.Services
{
    public class PackageService
    {
        private readonly MainDBContext _context;

        public PackageService(MainDBContext context)
        {
            _context = context;
        }

        public async Task<List<Package>> GetAvailablePackagesAsync(string userid)
        {
            try
            {
                User user = new User();
                user = _context.Users.Where(u => u.Id == Guid.Parse(userid)).FirstOrDefault();
                if(user != null)
                {
                    return await _context.Packages.Where(p => p.Country == user.CountryCode && p.IsActive == true).ToListAsync();
                }
                return new List<Package>();
            }
            catch(Exception ex)
            {
                return new List<Package>();
            }
           
        }

        public async Task<bool> BuyPackage(string userid, string packageId)
        {
            try
            {
                User user = new User();
                user = _context.Users.Where(u => u.Id == Guid.Parse(userid)).FirstOrDefault();
                if (user != null)
                {
                    var package =  _context.Packages.Where(p => p.Country == user.CountryCode && p.IsActive == true && p.Id == Guid.Parse(packageId)).ToListAsync();
                    if(AddPaymentCard())
                    {
                        if(PaymentCharge())
                        {
                            var userBooking = new UserPackage
                            {
                                Id = Guid.NewGuid(),
                                UserId = Guid.Parse(userid),
                                PackageId = Guid.Parse(packageId),
                                PurchaseDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                RemainingCredits = 50

                            };

                            _context.UserPackages.Add(userBooking);                           
                            await _context.SaveChangesAsync();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<List<UserPackage>> GetUserPackagesAsync(string userId)
        {
            try
            {
                return await _context.UserPackages
            .Where(up => up.UserId == Guid.Parse(userId))  
            .Include(up => up.Package)         
            .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<UserPackage>();
            }

        }

        public bool AddPaymentCard()
        {
            return true;
        }



        public bool PaymentCharge()
        {
            return true;
        }


    }
}
