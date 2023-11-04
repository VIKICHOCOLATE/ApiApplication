using Grpc.Core;
using Grpc.Net.Client;
using ProtoDefinitions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ApiApplication.Shared.Utilities;
using Microsoft.Extensions.Configuration;

namespace ApiApplication.Features.Movies.Services
{
	public class MoviesGrpcService : MoviesApi.MoviesApiBase
	{
		private readonly ILogger<MoviesGrpcService> _logger;
		private readonly MoviesApi.MoviesApiClient _client;

		private const string BaseAddressConfigurationPath = "Services:ExternalMovies:gRPC";


		public MoviesGrpcService(ILogger<MoviesGrpcService> logger, IConfiguration configuration)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			var basePath = configuration[BaseAddressConfigurationPath];
			var channel = GrpcChannel.ForAddress(basePath);
			_client = new MoviesApi.MoviesApiClient(channel);
		}

		public override async Task<responseModel> GetAll(Empty request, ServerCallContext context)
		{
			try
			{
				// Calling the gRPC service to fetch all movies
				var moviesResponse = await _client.GetAllAsync(request);
				if (moviesResponse != null && moviesResponse.Data.Is(showListResponse.Descriptor))
				{
					var shows = moviesResponse.Data.Unpack<showListResponse>();
					if (shows.Shows.Any())
					{
						return moviesResponse;  // Directly returning the fetched response if it has data
					}

					return GetResponseModel(false, ErrorMessages.Movies.NoMoviesFoundError, (int) StatusCode.NotFound);
				}
				return GetResponseModel(false, ErrorMessages.Movies.UnexpectedDataReturned, (int) StatusCode.Unknown);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex.ToString());
				return GetResponseModel(false, ex.Message, (int)StatusCode.Internal);
			}
		}

		public override async Task<responseModel> GetById(IdRequest request, ServerCallContext context)
		{
			try
			{
				var movieResponse = await _client.GetByIdAsync(request);
				if (movieResponse != null && movieResponse.Data.Is(showResponse.Descriptor))
				{
					return movieResponse;
				}

				return new responseModel
				{
					Success = false,
					Exceptions = { new moviesApiException { Message = ErrorMessages.Movies.NoMovieForId, StatusCode = (int)StatusCode.NotFound } }
				};
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex.ToString());
				return new responseModel
				{
					Success = false,
					Exceptions = { new moviesApiException { Message = ex.Message, StatusCode = (int)StatusCode.Internal } }
				};
			}
		}

		public override async Task<responseModel> Search(SearchRequest request, ServerCallContext context)
		{
			try
			{
				var moviesResponse = await _client.SearchAsync(request);
				if (moviesResponse != null && moviesResponse.Data.Is(showListResponse.Descriptor))
				{
					return moviesResponse;
				}

				return GetResponseModel(false, ErrorMessages.Movies.NoMoviesFoundedForSearchText, (int)StatusCode.NotFound);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex.ToString());
				return GetResponseModel(false, ex.Message, (int)StatusCode.Internal);
			}
		}

		private responseModel GetResponseModel(bool isSuccess, string errorMessage, int statusCode)
		{
			var response = new responseModel
			{
				Success = isSuccess,
				Exceptions = { new moviesApiException
					{
						Message = errorMessage,
						StatusCode = statusCode
					}}};

			return response;
		}
	}
}