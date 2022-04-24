using Maets.Models.Dtos.Reviews;
using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Apps;

public class AppReviewsDto : EntityDto
{
    public int AverageScore { get; set; }

    public ICollection<AppReviewScoresGroup> ScoresData = new List<AppReviewScoresGroup>();

    public ICollection<ReviewReadDto> Reviews { get; set; } = new List<ReviewReadDto>();

    public class AppReviewScoresGroup
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double AverageScore { get; set; }
    }
}
