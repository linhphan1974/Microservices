using BookOnline.MvcClient.Services;
using BookOnline.MvcClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.MvcClient.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? pageIndex, int? pageSize, int? typeId, int? catalogId, bool? isAvailable, [FromQuery] string errMessage)
        {
            var vm = await GetBooks(pageIndex, pageSize, typeId, catalogId, isAvailable);
            ViewBag.BasketInoperativeMsg = errMessage;
            return View(vm);
        }

        private async Task<BooksViewModel> GetBooks(int? pageIndex, int? pageSize, int? typeId, int?catalogId, bool? isAvailable)
        {
            int pages = pageSize ?? 10;
            var viewModel = await _bookService.GetBooksAsync(pageIndex ?? 0, pages, typeId, catalogId, isAvailable);

            BooksViewModel book = new BooksViewModel
            {
                BookCatalogs = await _bookService.GetBookCatalogsAsync(),
                BookTypes = await _bookService.GetBookTypesAsync(),
                BookItems = viewModel.Data.ToList(),
                SelectedType = (typeId.HasValue) ? await _bookService.GetBookTypeByIdAsync(typeId.Value) : null,
                SelectedCatalog = (catalogId.HasValue) ? await _bookService.GetBookCatalogByIdAsync(catalogId.Value) : null,
                PaginationInfo = new PaginationViewModel
                {
                    ActualPage = pageIndex ?? 0,
                    TotalItems = viewModel.Count,
                    ItemsPerPage = pageSize ?? 10,
                    TotalPages = (int)Math.Ceiling(((decimal)viewModel.Count / pages))
                }
            };

            book.PaginationInfo.Next = (book.PaginationInfo.ActualPage == book.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            book.PaginationInfo.Previous = (book.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return book;
        }
    }
}
