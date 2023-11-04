using ApiApplication.Features.Seats.Controllers;
using ApiApplication.Features.Seats.DTOs;
using ApiApplication.Features.Seats.Models;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class SeatsControllerTests
{
	private readonly Mock<ISeatsService> _seatsServiceMock;
	private readonly SeatsController _controller;

	public SeatsControllerTests(Mock<ISeatsService> seatsServiceMock)
	{
		_seatsServiceMock = seatsServiceMock;
		_controller = new SeatsController(_seatsServiceMock.Object);
	}

	[Fact]
	public async Task ReserveSeats_ReturnsBadRequest_WhenDesiredSeatsAreNull()
	{
		// Arrange
		List<SeatDto> seats = null;

		// Act
		var result = await _controller.ReserveSeats(1, seats);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
	}

	[Fact]
	public async Task ReserveSeats_ReturnsBadRequest_WhenDesiredSeatsAreEmpty()
	{
		// Arrange
		var seats = new List<SeatDto>();

		// Act
		var result = await _controller.ReserveSeats(1, seats);

		// Assert
		Assert.IsType<BadRequestObjectResult>(result);
	}

	[Fact]
	public async Task ReserveSeats_ReturnsOk_WhenReservationIsSuccessful()
	{
		// Arrange
		var seats = new List<SeatDto> { new SeatDto() };
		var reservationResponse = new ReservationResponse();
		_seatsServiceMock.Setup(x => x.ReserveSeats(1, seats))
			.ReturnsAsync((true, reservationResponse, null));

		// Act
		var result = await _controller.ReserveSeats(1, seats);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.Equal(reservationResponse, okResult.Value);
	}

	[Fact]
	public async Task ReserveSeats_ReturnsInternalServerError_WhenReservationFails()
	{
		// Arrange
		var seats = new List<SeatDto> { new SeatDto() };
		const string errorMessage = "An error occurred";
		_seatsServiceMock.Setup(x => x.ReserveSeats(1, seats))
			.ReturnsAsync((false, null, errorMessage));

		// Act
		var result = await _controller.ReserveSeats(1, seats);

		// Assert
		var objectResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(500, objectResult.StatusCode);

		// get the 'message' property
		var resultValue = objectResult.Value;
		var messageType = resultValue.GetType();
		var messageProperty = messageType.GetProperty("message");
		Assert.NotNull(messageProperty);

		var messageValue = messageProperty.GetValue(resultValue);
		Assert.Equal(errorMessage, messageValue);
	}

	[Fact]
	public async Task BuySeat_ReturnsOk_WhenPurchaseIsSuccessful()
	{
		// Arrange
		var reservationGuid = Guid.NewGuid();
		var ticket = new TicketDTO();
		_seatsServiceMock.Setup(x => x.BuySeat(reservationGuid))
			.ReturnsAsync((true, ticket, null));

		// Act
		var result = await _controller.BuySeat(reservationGuid);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.Equal(ErrorMessages.Seats.PurchaseConfirmation, okResult.Value);
	}


	[Fact]
	public async Task BuySeat_ReturnsBadRequest_WhenInvalidOperationExceptionOccurs()
	{
		// Arrange
		var reservationGuid = Guid.NewGuid();
		_seatsServiceMock.Setup(x => x.BuySeat(reservationGuid))
			.Throws(new InvalidOperationException("Invalid operation"));

		// Act
		var result = await _controller.BuySeat(reservationGuid);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid operation", badRequestResult.Value);
	}
}
