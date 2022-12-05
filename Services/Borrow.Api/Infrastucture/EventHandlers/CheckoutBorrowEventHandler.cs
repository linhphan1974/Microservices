using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using MediatR;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Infrastucture.EventHandlers
{
    public class CheckoutBorrowEventHandler : IEventHandler<CheckoutBorrowEvent>
    {
        private readonly ILogger<CheckoutBorrowEventHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IIdentityParser<ApplicationUser> _identityParser;
        public CheckoutBorrowEventHandler(ILogger<CheckoutBorrowEventHandler> logger,
            IMediator mediator,
            IIdentityParser<ApplicationUser> identityService)
        {
            _logger = logger;
            _mediator = mediator;
            _identityParser = identityService;
        }

        public async Task Handler(CheckoutBorrowEvent @event)
        {
            try
            {
                Address address = new Address();

                if (@event.ShipType == (int)ShipType.Shipping)
                {
                    address = new Address(
                        @event.User.State,
                        @event.User.City,
                        @event.User.PostalCode,
                        @event.User.ZipCode,
                        @event.User.Address,
                        @event.User.Email,
                        @event.User.PhoneNumber);
                }

                CreateBorrowCommand command = new CreateBorrowCommand(@event.User.Id, @event.User.UserName, address, @event.Basket, @event.ShipType);
                await _mediator.Send(command);

                _logger.LogInformation("Create borrow data successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Create borrow data fail!");
            }
        }
    }
}
