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
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("classschedule")]
        public async Task<IActionResult> GetAllClassSchedule([FromQuery] string userid)
        {
            try
            {
                var classSchedules = await _bookingService.GetAllClassScheduleAsync(userid);
                if (classSchedules == null || classSchedules.Count() == 0)
                {
                    return NotFound("There is no class schedule.");
                }
                else
                {
                    return Ok(classSchedules);
                }

            }
            catch (Exception ex)
            {
                return NotFound("There is no class schedule.");
            }

        }

        [HttpPost("book")]
        public async Task<IActionResult> BookClass([FromBody] BookClassRequest request)
        {
            try
            {
                var success = await _bookingService.BookClassAsync(request.userId, request.classScheduleId);
                if (success)
                    return Ok("Class booked successfully.");
                else
                    return BadRequest("Class is full. You have been added to the waitlist.");
            }
            catch (Exception ex)
            {
                return BadRequest("Booking class fail.");
            }
           
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking(CancelRequest cancelRequest)
        {

            // Fetch booking details
            var booking = await _bookingService.GetBookingByIdAsync(cancelRequest.bookingId);

            // Check if cancellation is within refund window
            if (Convert.ToDateTime(booking.BookingTime).AddHours(-4) > DateTime.UtcNow)
            {
                // Refund credits to user's package
            }

            var success = await _bookingService.CancelBookingAsync(cancelRequest.userId, cancelRequest.classScheduleId);
            if (success)
            {                
                return Ok("Class booked successfully.");
            }               
            else
                return BadRequest("Class is full. You have been added to the waitlist.");

            

            // Check if there are users on the waitlist and move the first in line to booked
        }


    }
}
