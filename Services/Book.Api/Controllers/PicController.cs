// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
using BookOnline.Book.Api.Infrastucture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookOnline.Book.Api.Controllers;

[ApiController]
public class PicController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly BookDBContext _context;

    public PicController(IWebHostEnvironment env,
        BookDBContext context)
    {
        _env = env;
        _context = context;
    }

    [HttpGet]
    [Route("api/book/items/{bookId:int}/pic")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    // GET: /<controller>/
    public async Task<ActionResult> GetImageAsync(int bookId)
    {
        if (bookId <= 0)
        {
            return BadRequest();
        }

        var item = await _context.BookItems
            .SingleOrDefaultAsync(ci => ci.Id == bookId);

        if (item != null)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, item.PictureUrl);

            string imageFileExtension = Path.GetExtension(item.PictureUrl);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimetype);
        }

        return NotFound();
    }

    private string GetImageMimeTypeFromImageFileExtension(string extension)
    {
        string mimetype;

        switch (extension)
        {
            case ".png":
                mimetype = "image/png";
                break;
            case ".gif":
                mimetype = "image/gif";
                break;
            case ".jpg":
            case ".jpeg":
                mimetype = "image/jpeg";
                break;
            case ".bmp":
                mimetype = "image/bmp";
                break;
            case ".tiff":
                mimetype = "image/tiff";
                break;
            case ".wmf":
                mimetype = "image/wmf";
                break;
            case ".jp2":
                mimetype = "image/jp2";
                break;
            case ".svg":
                mimetype = "image/svg+xml";
                break;
            default:
                mimetype = "application/octet-stream";
                break;
        }

        return mimetype;
    }
}
