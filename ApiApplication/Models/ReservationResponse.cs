namespace ApiApplication.Models
{
	public class ReservationResponse
	{
		public string ReservationGuid { get; set; }
		public int NumberOfSeats { get; set; }
		public int AuditoriumId { get; set; }
		public string MovieTitle { get; set; }
	}
}
