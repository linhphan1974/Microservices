using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Queries;
using BookOnline.Borrowing.Api.Models;
using BookOnline.Borrowing.Api.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQEventBus;

namespace BookOnline.Borrowing.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    //[Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BorrowController> _logger;
        private readonly IEventBus _eventBus;

        public BorrowController(IMediator mediator, ILogger<BorrowController> logger, IEventBus @event)
        {
            _mediator = mediator;
            _logger = logger;
            _eventBus = @event;
        }

        [HttpGet]
        [Route("items")]
        public async Task<List<BorrowDto>> GetBorrowsAsync(int pageIndex, int pageSize)
        {
            GetAllBorrowQuery query = new GetAllBorrowQuery(pageIndex, pageSize);
            var borrows = await _mediator.Send(query);
            return borrows;
        }

        [HttpGet]
        [Route("item/id/{id:int}")]
        public async Task<BorrowDto> GetByIdAsync(int id)
        {
            GetBorrowByIdQuery query = new GetBorrowByIdQuery(id);
            var borrow = await _mediator.Send(query);

            return borrow;
        }

        [HttpGet]
        [Route("items/alls/member/{memberid}")]
        public async Task<List<BorrowDto>> GetByMemberAsync(string memberId, int pageIndex, int pageSize)
        {
            var mem = await GetMemberByUserId(memberId);
            GetBorrowByMemberQuery query = new GetBorrowByMemberQuery(mem.Id, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }

        [HttpGet]
        [Route("items/status/{status:int}")]
        public async Task<List<BorrowDto>> GetByStatusAsync(int status, int pageIndex, int pageSize)
        {
            GetBorrowByStatusQuery query = new GetBorrowByStatusQuery(status, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);
            return borrows;
        }

        [HttpGet]
        [Route("items/memberid/{memberId}/status/{status:int}")]
        public async Task<List<BorrowDto>> GetBorrowsByMemberAndStatusAsync(string memberId, int status, int pageIndex, int pageSize)
        {
            var mem = await GetMemberByUserId(memberId);
            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression (mem.Id, status, null, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }
        [HttpGet]
        [Route("items/memberid/{memberId}/status/alls/date/{borrowDate}")]
        public async Task<List<BorrowDto>> GetBorrowsByMemberAndDateAsync(string memberId, DateTime borrowDate, int pageIndex, int pageSize)
        {
            var mem = await GetMemberByUserId(memberId);
            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression (mem.Id, null, borrowDate, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }
        [HttpGet]
        [Route("items/memberid/alls/status/{status:int}/date/{borrowDate}")]
        public async Task<List<BorrowDto>> GetBorrowsByStatusAndDateAsync(int status, DateTime borrowDate, int pageIndex, int pageSize)
        {
            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression (null, status, borrowDate, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }

        [HttpGet]
        [Route("items/memberid/{memberId}/status/{status:int}/date/{borrowDate}")]
        public async Task<List<BorrowDto>> GetBorrowsByMemberAndStatusAndDateAsync(string memberId, int status, DateTime borrowDate, int pageIndex, int pageSize)
        {
            var mem = await GetMemberByUserId(memberId);
            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression (mem.Id, status, borrowDate, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }

        [HttpGet]
        [Route("items/memberid/alls/status/{status:int}/date/alls")]
        public async Task<List<BorrowDto>> GetBorrowsByStatusAsync(int status, int pageIndex, int pageSize)
        {
            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression(null, status, null, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);
            return borrows;
        }

        [HttpGet]
        [Route("items/available/{memberid}")]
        public async Task<List<BorrowDto>> GetAvailableBorrowsAsync(string memberId, int pageIndex, int pageSize)
        {
            var mem = await GetMemberByUserId(memberId);
            GetBorrowByMemberQuery query = new GetBorrowByMemberQuery(mem.Id, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }

        [HttpGet]
        [Route("items/memberid/{memberId}/status/alls/date/alls")]
        public async Task<List<BorrowDto>> GetBorrowsByMemberAsync(string memberId, int pageIndex, int pageSize)
        {
            var mem = await GetMemberByUserId(memberId);

            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression(mem.Id, null, null, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }

        [HttpGet]
        [Route("items/memberid/alls/status/alls/date/{borrowDate}")]
        public async Task<List<BorrowDto>> GetBorrowsByDateAsync(DateTime borrowDate, int pageIndex, int pageSize)
        {
            GetBorrowsQueryByExpression query = new GetBorrowsQueryByExpression(null, null, borrowDate, pageIndex, pageSize);
            var borrows = await _mediator.Send(query);

            return borrows;
        }


        [HttpPut]
        [Route("return")]
        public async Task<ActionResult> SetBorrowToReturned(BorrowDto borrow)
        {
            BorrowChangeStatusToReturnCommand command = new BorrowChangeStatusToReturnCommand(borrow.Id);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut]
        [Route("pickup")]
        public async Task<ActionResult> SetBorrowToPickedUp(BorrowDto borrow)
        {
            BorrowChangeStatusToPickupCommand pickupCommand = new BorrowChangeStatusToPickupCommand(borrow.Id);
            var result = await _mediator.Send(pickupCommand);
            return Ok(result);
        }

        [HttpPut]
        [Route("waitforpickup")]
        public async Task<ActionResult> SetBorrowToWaitForPickup(BorrowDto borrow)
        {
            BorrowChangeStatusToWaitForPickupCommand command = new BorrowChangeStatusToWaitForPickupCommand(borrow.Id);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut]
        [Route("waitforship")]
        public async Task<ActionResult> SetBorrowToWaitForShip(BorrowDto borrow)
        {
            BorrowChangeStatusToShipCommand command = new BorrowChangeStatusToShipCommand(borrow.Id);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut]
        [Route("cancel")]
        public async Task<ActionResult> SetBorrowToCancel(BorrowDto borrow)
        { 
            BorrowCancelCommand pickupCommand = new BorrowCancelCommand(borrow.Id);
            var result = await _mediator.Send(pickupCommand);

            return Ok(result);
        }

        [HttpPut]
        [Route("ship")]
        public async Task<ActionResult> SetBorrowToShip(BorrowDto borrow)
        {
            BorrowChangeStatusToShipCommand shipCommand = new BorrowChangeStatusToShipCommand(borrow.Id);
            var result = await _mediator.Send(shipCommand);

            return Ok(result);
        }

        private async Task<MemberDto> GetMemberByUserId(string userId)
        {
            GetMemberByMemberIdQuery query = new GetMemberByMemberIdQuery(userId);
            var mem = await _mediator.Send(query);

            return mem;
        }
    }
}
