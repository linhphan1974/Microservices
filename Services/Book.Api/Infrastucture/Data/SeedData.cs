using Book.Api.Extension;
using BookOnline.Book.Api;
using BookOnline.Book.Api.Infrastucture.Data;
using BookOnline.Book.Api.Models;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Book.Api.Infrastucture.Data
{
    public class SeedData
    {
        public async Task SeedAsync(BookDBContext context, IWebHostEnvironment env, IOptions<BookSettings> settings, ILogger<SeedData> logger)
        {
            var policy = CreatePolicy(logger, nameof(SeedData));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;
                var picturePath = env.WebRootPath;

                if (!context.BookTypes.Any())
                {
                    await context.BookTypes.AddRangeAsync(GetBookTypeFromFile(contentRootPath,logger, context));
                    await context.SaveChangesAsync();
                }
                if (!context.BookCatalogs.Any())
                {
                    await context.BookCatalogs.AddRangeAsync(GetBookCatalogFromFile(contentRootPath,logger, context));
                    await context.SaveChangesAsync();
                }
                if (!context.BookItems.Any())
                {
                    await context.AddRangeAsync(GetBookItemFromFile(picturePath, contentRootPath, logger, context));
                    await context.SaveChangesAsync();
                    GetBookItemPictures(contentRootPath, picturePath);
                }
            });
        }

        #region Book Type
        private IEnumerable<BookType> GetBookTypeFromFile(string contentRootPath, ILogger<SeedData> logger, BookDBContext context)
        {
            string csvFileBookType = Path.Combine(contentRootPath, "Setup", "BookType.csv");

            if (!File.Exists(csvFileBookType))
            {
                return GetPreconfiguredBookType();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "name", "type" };
                csvheaders = GetBookTypeHeaders(csvFileBookType, requiredHeaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return GetPreconfiguredBookType();
            }

            var result = File.ReadAllLines(csvFileBookType)
                                        .Skip(1) // skip header row
                                        .Select(row=> Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                        .SelectTry(column => CreateBookType(column, csvheaders))
                                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null);

            return result;
        }

        private BookType CreateBookType(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            var bookType = new BookType()
            {
                Type = Convert.ToInt32(column[Array.IndexOf(headers, "type")].Trim('"').Trim()),
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim()
            };




            return bookType;
        }

        private string[] GetBookTypeHeaders(string csvfile, string[] requiredHeaders)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }


            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }


        private string[] GetHeaders(string csvFileCatalogBrands, string[] requiredHeaders)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<BookType> GetPreconfiguredBookType()
        {
            return new List<BookType>
            {
                new BookType{ Name="Book", Type=1},
                new BookType{ Name="E Book", Type=2},
            };
        }
        #endregion

        #region Book Catalog
        private IEnumerable<BookCatalog> GetBookCatalogFromFile(string contentRootPath, ILogger<SeedData> logger, BookDBContext context)
        {
            string csvFileBookCatalog = Path.Combine(contentRootPath, "Setup", "BookCatalog.csv");

            if (!File.Exists(csvFileBookCatalog))
            {
                return GetPreconfiguredBookCatalog();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "name", "description" };
                csvheaders = GetBookCatalogHeaders(csvFileBookCatalog, requiredHeaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return GetPreconfiguredBookCatalog();
            }

            return File.ReadAllLines(csvFileBookCatalog)
                                        .Skip(1) // skip header row
                                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                        .SelectTry(column => CreateBookCatalog(column, csvheaders))
                                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private BookCatalog CreateBookCatalog(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            var bookCatalog = new BookCatalog()
            {
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
                Description = column[Array.IndexOf(headers, "description")].Trim('"').Trim()
            };

            return bookCatalog;
        }

        private string[] GetBookCatalogHeaders(string csvfile, string[] requiredHeaders)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }


            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        private IEnumerable<BookCatalog> GetPreconfiguredBookCatalog()
        {
            return new List<BookCatalog>
            {
                new BookCatalog{ Name="History", Description="History book"},
                new BookCatalog{ Name="Scientic", Description="Scientic book"},
                new BookCatalog{ Name="Novel", Description="Novel book"},
                new BookCatalog{ Name="Medicine", Description="Medical book"},
                new BookCatalog{ Name="Engineering", Description="Engineering book"},
                new BookCatalog{ Name="Society", Description="Society book"},
                new BookCatalog{ Name="Politics", Description="Political book"},
                new BookCatalog{ Name="Literature", Description="Literature book"},
                new BookCatalog{ Name="Education", Description="Education book"}
            };
        }

        #endregion

        #region Book item
        private IEnumerable<BookItem> GetBookItemFromFile(string picturePath, string contentRootPath, ILogger<SeedData> logger, BookDBContext context)
        {
            string csvFileBookItems = Path.Combine(contentRootPath, "Setup", "BookItems.csv");

            if (!File.Exists(csvFileBookItems))
            {
                return GetPreconfiguredBookItems();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "ISBN", "FirstPublish", "Version", "Title", "Description", "CatalogId", "Publisher", "Author", "Status", "Quantity" };
                csvheaders = GetHeaders(csvFileBookItems, requiredHeaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return GetPreconfiguredBookItems();
            }

            var bookTypeIdLookup = context.BookTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
            var bookCatalogIdLookup = context.BookCatalogs.ToDictionary(ct => ct.Name, ct => ct.Id);

            return File.ReadAllLines(csvFileBookItems)
                        .Skip(1) // skip header row
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry(column => CreateBookItem(column, csvheaders, bookTypeIdLookup, bookCatalogIdLookup))
                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                        .Where(x => x != null);
        }

        private BookItem CreateBookItem(string[] column, string[] headers, Dictionary<int, int> bookTypeIdLookup, Dictionary<string, int> bookCatalogIdLookup)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            int bookTypeName = Convert.ToInt32(column[Array.IndexOf(headers, "name")].Trim('"').Trim());
            if (!bookTypeIdLookup.ContainsKey(bookTypeName))
            {
                throw new Exception($"type={bookTypeName} does not exist in book Types");
            }

            string bookCatalogName = column[Array.IndexOf(headers, "name")].Trim('"').Trim();
            if (!bookCatalogIdLookup.ContainsKey(bookCatalogName))
            {
                throw new Exception($"type={bookCatalogName} does not exist in book catalog");
            }

            string quantity =  column[Array.IndexOf(headers, "Quantity")].Trim('"').Trim();
            string publishDate = column[Array.IndexOf(headers, "FirstPublish")].Trim('"').Trim();
            string status = column[Array.IndexOf(headers, "Status")].Trim('"').Trim();

            if (!int.TryParse(quantity, out int quantityValue))
            {
                throw new Exception($"Quantity={quantity}is not a valid integer number");
            }

            if(!DateTime.TryParse(publishDate,out DateTime publishDateValue))
            {
                throw new Exception($"FirstPublish={publishDate}is not a valid date value");

            }

            if (!int.TryParse(status, out int statusValue))
            {
                throw new Exception($"Status={status}is not a valid integer number");
            }

            var bookItem = new BookItem()
            {
                BookTypeId = bookTypeIdLookup[bookTypeName],
                CatalogId = bookCatalogIdLookup[bookCatalogName],
                Description = column[Array.IndexOf(headers, "Description")].Trim('"').Trim(),
                Title = column[Array.IndexOf(headers, "Title")].Trim('"').Trim(),
                Quantity = quantityValue,
                Author = column[Array.IndexOf(headers, "Author")].Trim('"').Trim(),
                FirstPublish = publishDateValue,
                ISBN = column[Array.IndexOf(headers, "ISBN")].Trim('"').Trim(),
                Status = statusValue,
                Publisher = column[Array.IndexOf(headers, "Publisher")].Trim('"').Trim(),
                Version = column[Array.IndexOf(headers, "Version")].Trim('"').Trim()
            };



            return bookItem;
        }

        private IEnumerable<BookItem> GetPreconfiguredBookItems()
        {
            return new List<BookItem>{
                new BookItem { Author="Linh Phan", BookTypeId=1, CatalogId=5, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-3", Publisher="ABC corporation", Quantity=100, Status=1, Title="Software testing", Version=""},
                new BookItem { Author="Smit", BookTypeId=1, CatalogId=7, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-4", Publisher="DEF corporation", Quantity=20, Status=1, Title="Xoviet stories", Version=""},
                new BookItem { Author="Jennie", BookTypeId=1, CatalogId=3, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-5", Publisher="MMM corporation", Quantity=2000, Status=1, Title="Secret Garden", Version=""},
                new BookItem { Author="Max", BookTypeId=2, CatalogId=4, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-6", Publisher="NNN corporation", Quantity=50, Status=1, Title="Obisity", Version=""},
                new BookItem { Author="John", BookTypeId=1, CatalogId=8, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-7", Publisher="BCD corporation", Quantity=1, Status=1, Title="Gone with the win", Version=""},
                new BookItem { Author="Alice", BookTypeId=1, CatalogId=1, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-8", Publisher="ABC corporation", Quantity=10, Status=1, Title="Would war two", Version=""},
                new BookItem { Author="Peter", BookTypeId=2, CatalogId=6, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-9", Publisher="KKK corporation", Quantity=20, Status=1, Title="The development of human ", Version=""},
                new BookItem { Author="Adam", BookTypeId=1, CatalogId=2, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-0", Publisher="NNN corporation", Quantity=40, Status=1, Title="The universe", Version=""},
                new BookItem { Author="Mitchel", BookTypeId=1, CatalogId=9, Description="", FirstPublish=new DateTime(2000,1,1), ISBN="123-4-567-89012-1", Publisher="DEF corporation", Quantity=5, Status=1, Title="English In Use", Version=""},
            };
        }
        #endregion

        private void GetBookItemPictures(string contentRootPath, string picturePath)
        {
            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileCatalogItemPictures = Path.Combine(contentRootPath, "Setup", "BookItem.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<SeedData> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}

