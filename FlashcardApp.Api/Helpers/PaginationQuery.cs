using System.ComponentModel;

namespace FlashcardApp.Api.Helpers
{
    public class PaginationQuery
    {
        [FromQuery(Name = "pageNumber")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        [DefaultValue(int.MaxValue)]
        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 0")]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "skip")]
        [DefaultValue(0)]
        [Range(0, int.MaxValue, ErrorMessage = "Skip must be greater than or equal to 0")]
        public int Skip { get; set; } = 0;
    }
}