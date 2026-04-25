namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]?
    [AllowAnonymous]
    public class PaymentsController(IPaymentService paymentService) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;

        /// <summary>
        /// Retrieves all payments for a specific lease.
        /// </summary>
        [HttpGet("lease/{leaseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLease(Guid leaseId)
        {
            var payments = await _paymentService.GetPaymentsByLeaseAsync(leaseId);
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves a payment by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found" });
            }

            return Ok(payment);
        }

        /// <summary>
        /// Creates a new payment.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PaymentDto dto)
        {
            var payment = await _paymentService.CreatePaymentAsync(dto);
            if (payment == null)
            {
                return BadRequest(new { message = "Payment failed" });
            }

            return Ok(payment);
        }
    }
}
