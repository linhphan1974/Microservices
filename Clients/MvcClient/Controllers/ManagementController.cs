using AutoMapper;
using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using BookOnline.MvcClient.ViewModels;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.MvcClient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagementController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly IViewRenderService _viewRenderService;
        private readonly int _defaultPageSize = 20;
        private readonly IMapper _mapper;

        public ManagementController(IBorrowService borrowService, IViewRenderService viewRenderService, IMapper mapper)
        {
            _borrowService = borrowService;
            _viewRenderService = viewRenderService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(BorrowFilterViewModel filterModel)
        {
            var borrows = await _borrowService.GetBorrowsAsync(null, filterModel.Status, filterModel.BorrowDate, filterModel.SearchModel.PageIndex, filterModel.SearchModel.PageSize);
            var vm = new BorrowSearchViewModel
            {
                Borrows = borrows.ToList(),
                Status = (int)BorrowStatus.Confirmed,
                BorrowDate = DateTime.Now.Date,
                Pagination = new PagingModel {
                    PageIndex = borrows.PageNumber,
                    PageSize = borrows.PageSize,
                    TotalRows = borrows.TotalItemCount,
                    TotalPages = borrows.PageCount,
                }
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_BorrowBody", vm.Borrows) });

            }
            else
            {
                return View(vm);
            }
        }

        public async Task<JsonResult> Pickup(int borrowId, BorrowFilterViewModel filterModel)
        {
            var result = await _borrowService.Pickup(borrowId);

            if (result)
            {
                BorrowSearchViewModel vm = await GetBorrowSearch(filterModel);
                return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_BorrowBody", vm.Borrows) });
            }

            return Json(new { Success = false });
        }

        public async Task<JsonResult> Return(int borrowId, BorrowFilterViewModel filterModel)
        {
            var result = await _borrowService.Return(borrowId);

            if(result)
            {
                BorrowSearchViewModel vm = await GetBorrowSearch(filterModel);
                 return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_BorrowBody", vm.Borrows) });
           }
            return Json(new { Success = false });
        }

        public async Task<JsonResult> WaitForPickup(int borrowId, BorrowFilterViewModel filterModel)
        {
            var result = await _borrowService.SetWaitForPickup(borrowId);

            if (result)
            {
                BorrowSearchViewModel vm = await GetBorrowSearch(filterModel);
                return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_BorrowBody", vm.Borrows) });
            }
            return Json(new { Success = false });
        }
        public async Task<JsonResult> WaitForShip(int borrowId, BorrowFilterViewModel filterModel)
        {
            var result = await _borrowService.SetWaitForShip(borrowId);

            if (result)
            {
                BorrowSearchViewModel vm = await GetBorrowSearch(filterModel);
                return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_BorrowBody", vm.Borrows) });
            }
            return Json(new { Success = false });
        }
        //public async Task<ActionResult> Cancel(int Id)
        //{
        //    var result = await _borrowService.Cancel(Id);
            

        //    return RedirectToAction("Index");
        //}
        public async Task<JsonResult> Cancel(int borrowId, BorrowFilterViewModel filterModel)
        {
            var result = await _borrowService.Cancel(borrowId);

            if (result)
            {
                BorrowSearchViewModel vm = await GetBorrowSearch(filterModel);
                return Json(new { Success = true, View = await _viewRenderService.RenderToStringAsync("_BorrowBody", vm.Borrows) });
            }
            return Json(new { Success = false });
        }
        private async Task<BorrowSearchViewModel> GetBorrowSearch(BorrowFilterViewModel filterModel)
        {
            var borrows = await _borrowService.GetBorrowsAsync(null, filterModel.Status, filterModel.BorrowDate, filterModel.SearchModel.PageIndex, filterModel.SearchModel.PageSize);

            var vm = new BorrowSearchViewModel
            {
                Borrows = borrows.ToList(),
                Status = (int)BorrowStatus.Confirmed,
                BorrowDate = DateTime.Now.Date,
                Pagination = new PagingModel
                {
                    PageIndex = borrows.PageNumber,
                    PageSize = borrows.PageSize,
                    TotalRows = borrows.TotalItemCount,
                    TotalPages = borrows.PageCount,
                }
            };

            return vm;
        }
    }
}
