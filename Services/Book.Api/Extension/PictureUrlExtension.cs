using BookOnline.Book.Api.Models;

namespace BookOnline.Book.Api.Extension
{
    public static class PictureUrlExtension
    {
        public static void FillProductUrl(this BookItem item, string picBaseUrl)
        {
            if (item != null)
            {
                item.PictureUrl = picBaseUrl.Replace("[0]", item.Id.ToString());
            }
        }

    }
}
