namespace ApiApplication.Utilities
{
	public static class ErrorMessages
	{
		public static class ShowTimes
		{
			public const string EmptyExternalMovieId = "External movie ID cannot be empty or null.";
			public const string InternalServerError = "An internal server error occurred.";
		}

		public static class SeatsService
		{
			public const string ShowtimeNotFound = "Showtime not found.";
			public const string ConcurrencyConflictError = "A concurrency conflict occurred. Please try your operation again.";
			public const string InvalidReservation = "Invalid reservation.";
		}
	}
}
