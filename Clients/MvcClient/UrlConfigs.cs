namespace BookOnline.MvcClient
{
    public static class UrlConfigs
    {
        //========== Basket url ========
        public static class Basket {
            public static string GetBasketByIdUrl(string baseUri, string id) => $"{baseUri}/item/{id}";
            public static string UpdateBasketUrl(string baseUri) => $"{baseUri}/update";
            public static string AddToCart(string baseUri) => $"{baseUri}/addtocart";
            public static string DeleteBasketItemUrl(string baseUri, int id) => $"{baseUri}/items/{id}";
            public static string ClearBasketUrl(string baseUri, string basketId) => $"{baseUri}/{basketId}";
            public static string Checkout(string baseUri) => $"{baseUri}/checkout";

        }

        public static class Book
        {
            //========== Book Url ==========
            public static string GetBooksByTypeAndCatalogUrl(string baseUri, int page, int take, int? type, int? catalog, bool? isAvailable)
            {
                string filterQs = "/type/{0}/catalog/{1}/available/{2}";
                string typeFilter = string.Empty;
                string catalogFilter = string.Empty;
                string boolFilter = string.Empty;

                typeFilter = (type.HasValue) ? type.Value.ToString() : "alltype";

                catalogFilter = (catalog.HasValue) ? catalog.Value.ToString() : "allcatalog";

                boolFilter = (isAvailable.HasValue) ? isAvailable.Value.ToString().ToLower() : string.Empty;

                filterQs = string.Format(filterQs, typeFilter, catalogFilter, boolFilter);

                return $"{baseUri}items{filterQs}?pageIndex={page}&pageSize={take}";

            }

            public static string GetBookItemsUrl(string baseUri, int page, int take, List<int>? ids)
            {
                string result = "";
                if (ids is not null)
                {
                    result = $"/ids/{ids}";
                }

                return $"{baseUri}{result}?pageIndex={page}&pageSize={take}";
            }
            public static string GetBookByIdUrl(string baseUri, int id) => $"{baseUri}item/{id}";

            public static string GetBookTypesUrl(string baseUri) => $"{baseUri}types";
            public static string GetBookTypeUrl(string baseUri, int id) => $"{baseUri}types/{id}"; 
            public static string GetBookCatalogsUrl(string baseUri) => $"{baseUri}catalogs";
            public static string GetBookCatalogUrl(string baseUri, int id) => $"{baseUri}catalog/{id}";
        }

        public static class Aggregator
        {
            public static string GetBasketByIdUrl(string baseUri, string id) => $"{baseUri}/api/basket/{id}";
            public static string UpdateBasketUrl(string baseUri) => $"{baseUri}/api/basket/update";
            public static string AddToCart(string baseUri) => $"{baseUri}/api/basket/addtocart";
            public static string GetDraftBorrow(string baseUri, string memberId) => $"{baseUri}/api/borrow/{memberId}";
        }

        public static class Borrow
        {
            public static string GetBorrowsUrl(string baseUri, string memberId, int? status, DateTime? borrowDate, int pageIndex, int pageSize)
            {
                string url = "/items/memberid/{0}/status/{1}/date/{2}";
                string statusFilter = status.HasValue ? status.Value.ToString() : "alls";
                string memberFilter = !string.IsNullOrEmpty(memberId) ? memberId : "alls";
                string dateFilter = borrowDate.HasValue ? borrowDate.Value.Date.ToString("U") : "alls";

                url = string.Format(url, memberFilter, statusFilter, dateFilter);

                return $"{baseUri}{url}?pageIndex={pageIndex}&pageSize={pageSize}";
            }

            public static string GetAvailableBorrowsUrl(string baseUri, string memberId, int pageIndex, int pageSize) => $"{baseUri}/items/available/{memberId}?pageIndex={pageIndex}&pageSize={pageSize}";
            public static string GetWaitForPickupUrl(string baseUri) => $"{baseUri}/waitforpickup";
            public static string GetPickupUrl(string baseUri) => $"{baseUri}/pickup";
            public static string GetWaitForShipUrl(string baseUri) => $"{baseUri}/waitforship";
            public static string GetShipUrl(string baseUri) => $"{baseUri}/ship";
            public static string GetReturnUrl(string baseUri) => $"{baseUri}/return";
            public static string GetCancelUrl(string baseUri) => $"{baseUri}/cancel";
        }
    }
}
