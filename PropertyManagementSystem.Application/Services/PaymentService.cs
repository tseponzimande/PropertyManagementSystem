namespace PropertyManagementSystem.Application.Services
{
    public class PaymentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PaymentService> logger) : IPaymentService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<PaymentService> _logger = logger;

        #endregion

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByLeaseAsync(Guid leaseId)
        {
            var payments = await _unitOfWork.Payments.GetPaymentsByLeaseAsync(leaseId);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto?> CreatePaymentAsync(PaymentDto dto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(dto);
                payment.Id = Guid.NewGuid();
                //payment.Status = "Completed";
                payment.Status = PaymentEnum.Completed.ToString();
                payment.PaymentDate = DateTime.UtcNow;

                await _unitOfWork.Payments.CreateAsync(payment);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<PaymentDto>(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment");
                return null;
            }
        }
    }
}