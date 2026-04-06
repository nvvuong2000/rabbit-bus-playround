using FormulaAirline.API.Model;
using FormulaAirline.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormulaAirline.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController: ControllerBase
    {
        private readonly ILogger<BookingsController> _logger;
        private readonly IMessageProducer _messageProducer;
        public BookingsController(ILogger<BookingsController> logger, IMessageProducer messageProducer)
        {
            logger = _logger;
            _messageProducer = messageProducer;
        }

        public static readonly List<Booking> _bookings = new();

        [HttpPost]
        public IActionResult CreateBooking(Booking newBooking)
        {
            if(!ModelState.IsValid) return BadRequest();
            _bookings.Add(newBooking);
            _messageProducer.SendingMessage<Booking>(newBooking);
            return Ok();
        }
    }
}
