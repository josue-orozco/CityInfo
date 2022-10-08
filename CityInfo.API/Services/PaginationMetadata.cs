namespace CityInfo.API.Services
{
    // this is the preferred way to add pagination metadata
    // some APIs send metadat via response body but then you return a 
    // Json object with metadata not only the Json object asked for
    // we will be sending it in header through a custom header tag
    public class PaginationMetadata
    {
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}
