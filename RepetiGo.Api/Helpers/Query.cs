using System.ComponentModel;

namespace RepetiGo.Api.Helpers
{
    public class Query
    {
        [FromQuery(Name = "filter")]
        [DefaultValue(null)]
        public string? Filter { get; set; }

        [FromQuery(Name = "include")]
        [DefaultValue(null)]
        public string? Include { get; set; }

        [FromQuery(Name = "sort")]
        [DefaultValue(null)]
        public string? Sort { get; set; }

        [FromQuery(Name = "descending")]
        [DefaultValue(false)]
        public bool Descending { get; set; } = false;

        [FromQuery(Name = "page")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "size")]
        [DefaultValue(int.MaxValue)]
        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 0")]
        public int Size { get; set; } = int.MaxValue;

        [FromQuery(Name = "skip")]
        [DefaultValue(0)]
        [Range(0, int.MaxValue, ErrorMessage = "Skip must be greater than or equal to 0")]
        public int Skip { get; set; } = 0;
    }
}