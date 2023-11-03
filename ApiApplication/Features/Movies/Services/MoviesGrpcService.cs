using Grpc.Core;
using Grpc.Net.Client;
using ProtoDefinitions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Features.Movies.Services
{
	public class MoviesGrpcService : MoviesApi.MoviesApiBase
	{
		private readonly ILogger<MoviesGrpcService> _logger;
		private readonly GrpcChannel _channel;
		private readonly MoviesApi.MoviesApiClient _client;

		private const string BaseAddress = "https://localhost:7443";

		public MoviesGrpcService(ILogger<MoviesGrpcService> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_channel = GrpcChannel.ForAddress(BaseAddress);
			_client = new MoviesApi.MoviesApiClient(_channel);
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
					else
					{
						return new responseModel
						{
							Success = false,
							Exceptions = { new moviesApiException { Message = "No movies found.", StatusCode = (int)StatusCode.NotFound } }
						};
					}
				}
				else
				{
					return new responseModel
					{
						Success = false,
						Exceptions = { new moviesApiException { Message = "Unexpected data type returned or no data available.", StatusCode = (int)StatusCode.Unknown } }
					};
				}
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

		public override async Task<responseModel> GetById(IdRequest request, ServerCallContext context)
		{
			try
			{
				var movieResponse = await _client.GetByIdAsync(request);
				if (movieResponse != null && movieResponse.Data.Is(showResponse.Descriptor))
				{
					return movieResponse;
				}
				else
				{
					return new responseModel
					{
						Success = false,
						Exceptions = { new moviesApiException { Message = "No movie found for the provided ID.", StatusCode = (int)StatusCode.NotFound } }
					};
				}
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
				else
				{
					return new responseModel
					{
						Success = false,
						Exceptions = { new moviesApiException { Message = "No movies found for the provided search text.", StatusCode = (int)StatusCode.NotFound } }
					};
				}
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
	}
}