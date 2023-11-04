using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;
using ApiApplication.Features.Movies.Services;
using ApiApplication.Features.Movies.DTOs;
using ApiApplication.Shared.Services;

public class ExternalMovieServiceTests
{
	private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
	private readonly Mock<ILogger<ExternalMovieService>> _loggerMock;
	private readonly Mock<IConfiguration> _configMock;
	private readonly Mock<ICacheService> _cacheServiceMock;

	public ExternalMovieServiceTests(Mock<IHttpClientFactory> httpClientFactoryMock, 
		Mock<ILogger<ExternalMovieService>> loggerMock, Mock<IConfiguration> configMock, Mock<ICacheService> cacheServiceMock)
	{
		_httpClientFactoryMock = httpClientFactoryMock;
		_loggerMock = loggerMock;
		_configMock = configMock;
		_cacheServiceMock = cacheServiceMock;
		_configMock.Setup(c => c["Services:ExternalMovies:ApiKey"]).Returns("test_api_key");
	}

	[Fact]
	public async Task GetByIdAsync_ReturnsDataFromApi_WhenCacheIsEmpty()
	{
		// Arrange
		var movieId = "123";
		var baseAddress = "http://fakeapi.com/";
		var expectedMovie = new ExternalMovieDto { Id = movieId, Title = "Test Movie" };
		var expectedJson = JsonSerializer.Serialize(expectedMovie);

		var mockResponseMessage = new HttpResponseMessage
		{
			StatusCode = HttpStatusCode.OK,
			Content = new StringContent(expectedJson)
		};

		var mockHttpMessageHandler = new MockHttpMessageHandler(mockResponseMessage);
		var httpClient = new HttpClient(mockHttpMessageHandler)
		{
			BaseAddress = new Uri(baseAddress)
		};

		_httpClientFactoryMock.Setup(factory => factory.CreateClient("ExternalMovies"))
			.Returns(httpClient);

		_cacheServiceMock.Setup(service => service.GetCachedDataAsync<ExternalMovieDto>(It.Is<string>(s => s == $"Movie_{movieId}")))
			.ReturnsAsync((ExternalMovieDto)null);

		var service = new ExternalMovieService(
			_httpClientFactoryMock.Object,
			_configMock.Object,
			_loggerMock.Object,
			_cacheServiceMock.Object
		);

		// Act
		var result = await service.GetByIdAsync(movieId);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Movie);
		Assert.Equal(movieId, result.Movie.Id);
		Assert.Equal("Test Movie", result.Movie.Title);

		_cacheServiceMock.Verify(service => service.SetCacheDataAsync(It.Is<string>(s => s == $"Movie_{movieId}"), It.IsAny<ExternalMovieDto>(), It.IsAny<TimeSpan>()), Times.Once);
	}


	[Fact]
	public async Task GetAllAsync_ReturnsDataFromCache_WhenCacheIsNotEmpty()
	{
		// Arrange
		var expectedMovies = new ExternalMovieListDto
		{
			Movies = new List<ExternalMovieDto>()
			{
				new ExternalMovieDto { Id = "123", Title = "Test Movie 1" },
				new ExternalMovieDto { Id = "456", Title = "Test Movie 2" },
			}
		};

		_cacheServiceMock.Setup(service => service.GetCachedDataAsync<ExternalMovieListDto>(It.IsAny<string>()))
			.ReturnsAsync(expectedMovies);

		var service = new ExternalMovieService(
			_httpClientFactoryMock.Object,
			_configMock.Object,
			_loggerMock.Object,
			_cacheServiceMock.Object
		);

		// Act
		var result = await service.GetAllAsync();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Movies);
		Assert.Equal(2, result.Movies.Movies.Count);

		_httpClientFactoryMock.Verify(factory => factory.CreateClient(It.IsAny<string>()), Times.Never);
	}

	public class MockHttpMessageHandler : DelegatingHandler
	{
		private readonly HttpResponseMessage _mockResponse;

		public MockHttpMessageHandler(HttpResponseMessage responseMessage)
		{
			_mockResponse = responseMessage;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(_mockResponse);
		}
	}
}
