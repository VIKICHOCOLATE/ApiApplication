namespace ApiApplication.Shared.Utilities
{
    public static class ErrorMessages
    {
        public static class ShowTimes
        {
            public const string EmptyExternalMovieId = "External movie ID cannot be empty or null.";
            public const string InternalServerError = "An internal server error occurred.";
        }

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
    }
}
