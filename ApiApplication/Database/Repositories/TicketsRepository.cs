using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Database.Repositories
{
	public class TicketsRepository : ITicketsRepository
	{
		private readonly CinemaContext _context;

		public TicketsRepository(CinemaContext context)
		{
			_context = context;
		}

		public Task<TicketEntity> GetAsync(Guid id, CancellationToken cancel)
		{
			return _context.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancel);
		}

		public async Task<IEnumerable<TicketEntity>> GetEnrichedAsync(int showtimeId, CancellationToken cancel)
		{
			return await _context.Tickets
				.Include(x => x.Showtime)
				.Include(x => x.Seats)
				.Where(x => x.ShowtimeId == showtimeId)
				.ToListAsync(cancel);
		}

		public async Task<TicketEntity> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel)
		{
			var ticket = _context.Tickets.Add(new TicketEntity
			{
				Showtime = showtime,
				Seats = new List<SeatEntity>(selectedSeats)
			});

			try
			{
				await _context.SaveChangesAsync(cancel);
			}
			catch (DbUpdateConcurrencyException)
			{
				// Handle the exception, maybe rethrow with a user-friendly message
				throw new InvalidOperationException("The seat(s) you tried to reserve might have been taken. Please try again.");
			}

			return ticket.Entity;
		}

		public async Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel)
		{
			if (ticket.Paid)
			{
				throw new InvalidOperationException("This ticket has already been purchased.");
			}

			ticket.Paid = true;
			_context.Update(ticket);

			try
			{
				await _context.SaveChangesAsync(cancel);
			}
			catch (DbUpdateConcurrencyException)
			{
				// Handle the exception, maybe rethrow with a user-friendly message
				throw new InvalidOperationException("There was a problem confirming the payment. Please try again.");
			}

			return ticket;
		}

		public IDisposable BeginTransaction()
		{
			return _context.Database.BeginTransaction();
		}

		public void Commit()
		{
			_context.Database.CommitTransaction();
		}

		public void Rollback()
		{
			_context.Database.RollbackTransaction();
		}

	}
}
