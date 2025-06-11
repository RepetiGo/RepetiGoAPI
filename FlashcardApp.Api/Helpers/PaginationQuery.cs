using System.ComponentModel;

namespace FlashcardApp.Api.Helpers
{
    public class PaginationQuery
    {
        [FromQuery(Name = "pageNumber")]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 0")]
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "skip")]
        [Range(0, int.MaxValue, ErrorMessage = "Skip must be greater than or equal to 0")]
        [DefaultValue(0)]
        public int Skip { get; set; } = 0;
    }
}