namespace ApiApplication.Shared.Utilities
{
    public static class ErrorMessages
    {
        public static class Seats
        {
            public const string ShowtimeNotFound = "Showtime not found.";
            public const string ConcurrencyConflictError = "A concurrency conflict occurred. Please try your operation again.";
            public const string InvalidReservation = "Invalid reservation.";
            public const string EmptySeatsError = "Seats list cannot be empty.";
			public const string PurchaseConfirmation = "Seat successfully purchased.";
			public const string SeatsNotContiguousError = "Seats are not contiguous.";
			public const string SeatsAreSoldError = "One or more seats are already reserved or sold.";
        }

        public static class Movies
        {
	        public const string NoMoviesFoundError = "No movies found.";
	        public const string UnexpectedDataReturned = "Unexpected data type returned or no data available.";
	        public const string NoMovieForId = "No movie found for the provided ID.";
	        public const string NoMoviesFoundedForSearchText = "No movies found for the provided search text.";
		}
    }
}
