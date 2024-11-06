using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Services
{
    public class BookingService
    {
        private readonly MainDBContext _context;
        //private readonly ICacheService _cacheService;  // For caching

        public BookingService(MainDBContext context)
        {
            _context = context;
        }

        public async Task<List<ClassSchedule>> GetAllClassScheduleAsync(string userid)
        {
            try
            {
                User user = new User();
                user = _context.Users.Where(u => u.Id == Guid.Parse(userid)).FirstOrDefault();
                if (user != null)
                {
                    return await _context.ClassSchedules.Where(p => p.Country == user.CountryCode).ToListAsync();
                }
                return new List<ClassSchedule>();
            }
            catch (Exception ex)
            {
                return new List<ClassSchedule>();
            }

        }

        public async Task<UserBooking> GetBookingByIdAsync(string bookingid)
        {
            try
            {
                UserBooking booking = new UserBooking();
                booking = await _context.UserBookings.Where(u => u.Id == Guid.Parse(bookingid)).FirstOrDefaultAsync();
                if (booking != null)
                {
                    return booking;
                }
                return new UserBooking();
            }
            catch (Exception ex)
            {
                return new UserBooking();
            }

        }

        

        public async Task<bool> BookClassAsync(string userId, string classScheduleId)
        {
            // Check if the class is available
            var classSchedule = await _context.ClassSchedules
                .FirstOrDefaultAsync(cs => cs.Id == Guid.Parse(classScheduleId) && cs.StartTime > DateTime.Now);

            if (classSchedule == null || classSchedule.CurrentParticipants >= classSchedule.MaxParticipants)
            {
                // If class is full, add user to waitlist
                var waitlistEntry = new Waitlist
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    ClassScheduleId = Guid.Parse(classScheduleId),
                    WaitlistTime = DateTime.Now
                };
                _context.Waitlists.Add(waitlistEntry);
                await _context.SaveChangesAsync();
                return false;  // Class is full, user added to waitlist
            }

            // Ensure no overlap with other bookings
            var conflictingBooking = await _context.UserBookings
                .AnyAsync(ub => ub.UserId == Guid.Parse(userId) &&
                                ((ub.ClassSchedule.StartTime < classSchedule.EndTime && ub.ClassSchedule.EndTime > classSchedule.StartTime)));

            if (conflictingBooking)
            {
                throw new InvalidOperationException("You cannot book overlapping classes.");
            }

            // deduct 1 credit from remaining credit
            var userPackage = await _context.UserPackages
                .FirstOrDefaultAsync(cs => cs.UserId == Guid.Parse(userId) && cs.PackageId == classSchedule.packageId);

            if (userPackage == null)
            {
                throw new InvalidOperationException("There is no purchased package.");
            }
            else
            {
                userPackage.RemainingCredits--;
            }

            // Proceed with booking the class
            var userBooking = new UserBooking
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                ClassScheduleId = Guid.Parse(classScheduleId),
                BookingTime = DateTime.Now,
                IsCancelled = false,
            };

            classSchedule.CurrentParticipants++;
            _context.UserBookings.Add(userBooking);
            await _context.SaveChangesAsync();
            return true;  // Booking successful
        }



        public async Task<bool> CancelBookingAsync(string userId, string classScheduleId)
        {
            var booking = await _context.UserBookings
                .FirstOrDefaultAsync(ub => ub.UserId == Guid.Parse(userId) && ub.ClassScheduleId == Guid.Parse(classScheduleId) && !Convert.ToBoolean(ub.IsCancelled));

            if (booking == null)
                throw new InvalidOperationException("Booking not found or already cancelled.");

            booking.IsCancelled = true;
            var classSchedule = await _context.ClassSchedules
                .FirstOrDefaultAsync(cs => cs.Id == Guid.Parse(classScheduleId));

            if (classSchedule != null)
            {
                classSchedule.CurrentParticipants--;
                // Process FIFO for waitlist
                var nextWaitlistUser = await _context.Waitlists
                    .Where(w => w.ClassScheduleId == Guid.Parse(classScheduleId))
                    .OrderBy(w => w.WaitlistTime)
                    .FirstOrDefaultAsync();

                if (nextWaitlistUser != null)
                {
                    var userBooking = new UserBooking
                    {
                        Id = Guid.NewGuid(),
                        UserId = nextWaitlistUser.UserId,
                        ClassScheduleId = Guid.Parse(classScheduleId),
                        BookingTime = DateTime.Now,
                        IsCancelled = false
                    };

                    _context.UserBookings.Add(userBooking);
                    _context.Waitlists.Remove(nextWaitlistUser);  // Remove from waitlist
                    await _context.SaveChangesAsync();
                    return true;
                }
               
            }
            return false;
        }

      


    }
}
