using System;
using System.ComponentModel.DataAnnotations;

namespace ScrumMovieTheater.Models
{
    public class UpdateMovieViewModel
    {
        // Movie
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public string Genre { get; set; } = "";

        public int RuntimeMinutes { get; set; }

        public string Rating { get; set; } = "";

        public DateTime ReleaseDate { get; set; }

        public string ImageUrl { get; set; } = "";

        // Showtime
        public int ShowtimeId { get; set; }

        public int TheaterId { get; set; }

        public int AuditoriumId { get; set; }

        public DateTime ShowDate { get; set; }

        public TimeSpan TimeSlot { get; set; }

        public decimal Price { get; set; }
    }
}