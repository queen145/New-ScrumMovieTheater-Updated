using System.Collections.Generic;

namespace ScrumMovieTheater.Models
{
    public class HomeViewModel
    {
        public List<Movie> NowShowing { get; set; } = new List<Movie>();
        public List<Movie> ComingSoon { get; set; } = new List<Movie>();
    }
}