using ApiApplication.Database.Entities;
using System.Collections.Generic;
using System;

namespace ApiApplication.Features.Seats.DTOs
{
	public class TicketDTO
	{
		public TicketDTO()
		{
			CreatedTime = DateTime.Now;
			Paid = false;
		}

		public Guid Id { get; set; }
		public int ShowtimeId { get; set; }
		public DateTime CreatedTime { get; set; }
		public bool Paid { get; set; }
	}
}
