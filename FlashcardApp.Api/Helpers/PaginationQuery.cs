namespace FlashcardApp.Api.Helpers
{
    public class PaginationQuery
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = int.MaxValue;

        public int Skip { get; set; } = 0;
    }
}