using ApiApplication.Features.Seats.DTOs;
using ApiApplication.Features.Seats.Models;
using ApiApplication.Features.Seats.Services;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using ApiApplication.Database.Entities;
using ApiApplication.Shared.Utilities;

namespace ApiApplication.Tests.Services
{
	public class SeatsServiceTests
	{
		private readonly Mock<ITicketsRepository> _mockTicketsRepository;
		private readonly Mock<IShowtimesRepository> _mockShowtimesRepository;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILogger<SeatsService>> _mockLogger;
		private readonly SeatsService _service;

		public SeatsServiceTests()
		{
			_mockTicketsRepository = new Mock<ITicketsRepository>();
			_mockShowtimesRepository = new Mock<IShowtimesRepository>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILogger<SeatsService>>();

			_service = new SeatsService(
				_mockTicketsRepository.Object,
				_mockShowtimesRepository.Object,
				_mockMapper.Object,
				_mockLogger.Object);
		}

		[Fact]
		public async Task ReserveSeats_ReturnsSuccess_WhenAllConditionsMet()
		{
			// Arrange
			var showtimeId = 1;
			var desiredSeats = new List<SeatDto>
			{
				new SeatDto { AuditoriumId = 1, Row = 1, SeatNumber = 1 },
				new SeatDto { AuditoriumId = 1, Row = 1, SeatNumber = 2 }
			};

			var showtime = new List<ShowtimeEntity> { // Populate with a valid showtime entity
				new()
				{
					Id = 1,
					Movie = new MovieEntity
					{
						Id = "1",
						Title = "Test Movie"
					}
				}
			};
			var reservedTicket = new TicketEntity { 
				//Populate with expected reserved ticket details
				Id = Guid.NewGuid(),
				ShowtimeId = 1,
				Showtime = showtime.First(),
				Seats = new List<SeatEntity>
				{
					new() { Row = 1, SeatNumber = 1, AuditoriumId = 1 },
					new() { Row = 1, SeatNumber = 2, AuditoriumId = 1 }
				}
			};
			var reservationResponse = new ReservationResponse
			{
				ReservationGuid = reservedTicket.Id.ToString(),
				NumberOfSeats = desiredSeats.Count,
				AuditoriumId = desiredSeats[0].AuditoriumId,
				MovieTitle = showtime.First().Movie.Title
			};

			_mockShowtimesRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(), CancellationToken.None))
									.ReturnsAsync(showtime);
			_mockTicketsRepository.Setup(repo => repo.CreateAsync(showtime.First(), It.IsAny<List<SeatEntity>>(), CancellationToken.None))
								  .ReturnsAsync(reservedTicket);

		_mockMapper.Setup(m => m.Map<List<SeatEntity>>(It.IsAny<List<SeatDto>>()))
				.Returns(new List<SeatEntity>()
				{
					new SeatEntity { Row = 1, SeatNumber = 1, AuditoriumId = 1 },
					new SeatEntity { Row = 1, SeatNumber = 2, AuditoriumId = 1 }
				});

			var service = new SeatsService(_mockTicketsRepository.Object, _mockShowtimesRepository.Object, _mockMapper.Object, _mockLogger.Object);

			// Act
			var result = await _service.ReserveSeats(showtimeId, desiredSeats);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.ReservationResponse);
			Assert.Equal(reservationResponse.ReservationGuid, result.ReservationResponse.ReservationGuid);
			Assert.Equal(reservationResponse.NumberOfSeats, result.ReservationResponse.NumberOfSeats);
			Assert.Equal(reservationResponse.AuditoriumId, result.ReservationResponse.AuditoriumId);
			Assert.Equal(reservationResponse.MovieTitle, result.ReservationResponse.MovieTitle);
			Assert.Null(result.ErrorMessage);
		}


		[Fact]
		public async Task ReserveSeats_ReturnsError_WhenShowtimeNotFound()
		{
			// Arrange
			var showtimeId = 1; // Non-existent showtime ID for this test
			var desiredSeats = new List<SeatDto> {
				new SeatDto { AuditoriumId = 1, Row = 1, SeatNumber = 1 },
				new SeatDto { AuditoriumId = 1, Row = 1, SeatNumber = 2 }
			};

			// return an empty list to simulate not finding the showtime
			_mockShowtimesRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(), CancellationToken.None))
									.ReturnsAsync(new List<ShowtimeEntity>());
			var service = new SeatsService(_mockTicketsRepository.Object, _mockShowtimesRepository.Object, _mockMapper.Object, _mockLogger.Object);

			// Act
			var result = await _service.ReserveSeats(showtimeId, desiredSeats);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Null(result.ReservationResponse);
			Assert.Equal(ErrorMessages.Seats.ShowtimeNotFound, result.ErrorMessage);
		}


		[Fact]
		public async Task ReserveSeats_ReturnsError_WhenSeatsNotContiguous()
		{
			// Arrange
			var showtimeId = 1;
			var desiredSeats = new List<SeatDto>
			{
				new SeatDto { Row = 1, SeatNumber = 1, AuditoriumId = 1 },
				new SeatDto { Row = 1, SeatNumber = 3, AuditoriumId = 1 }
			};

			_mockShowtimesRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(), CancellationToken.None))
									.ReturnsAsync(new List<ShowtimeEntity> { 
										new ShowtimeEntity()
										{
											Movie = new MovieEntity
											{
												Id = "1",
												Title = "Test Movie"
											}

										}
									});

			var service = new SeatsService(_mockTicketsRepository.Object, _mockShowtimesRepository.Object, _mockMapper.Object, _mockLogger.Object);

			// Act
			var result = await _service.ReserveSeats(showtimeId, desiredSeats);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Null(result.ReservationResponse);
			Assert.Equal(ErrorMessages.Seats.SeatsNotContiguousError, result.ErrorMessage);
		}


		[Fact]
		public async Task BuySeat_ReturnsSuccess_WhenPaymentConfirmed()
		{
			// Arrange
			var reservationGuid = Guid.NewGuid();
			var mockTicket = new TicketEntity 
			{
				Id = reservationGuid,
				Showtime = new ShowtimeEntity
				{
					Movie = new MovieEntity
					{
						Id = "1",
						Title = "Test Movie"
					}
				}
			};

			_mockTicketsRepository.Setup(repo => repo.GetAsync(reservationGuid, CancellationToken.None))
								  .ReturnsAsync(mockTicket);
			_mockTicketsRepository.Setup(repo => repo.ConfirmPaymentAsync(mockTicket, CancellationToken.None))
								  .ReturnsAsync(mockTicket);

			_mockMapper.Setup(m => m.Map<TicketDTO>(It.IsAny<TicketEntity>()))
					   .Returns(new TicketDTO 
					   {
						   Id = reservationGuid,
						   ShowtimeId = 1
					   });

			var service = new SeatsService(_mockTicketsRepository.Object, _mockShowtimesRepository.Object, _mockMapper.Object, _mockLogger.Object);

			// Act
			var result = await _service.BuySeat(reservationGuid);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.ticket);
			Assert.Null(result.ErrorMessage);
		}
	}
}
