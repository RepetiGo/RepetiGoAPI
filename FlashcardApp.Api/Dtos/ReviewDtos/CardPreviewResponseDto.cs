namespace FlashcardApp.Api.Dtos.ReviewDtos
{
    public class CardPreviewResponseDto
    {
        public int IntervalDaysOfAgain { get; set; }

        public int IntervalDaysOfHard { get; set; }

        public int IntervalDaysOfGood { get; set; }

        public int IntervalDaysOfEasy { get; set; }
    }
}
