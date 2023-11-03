using Grpc.Core;
using Grpc.Net.Client;
using ProtoDefinitions;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiApplication.Features.Movies.Services
{
	public class MoviesApiService : MoviesApi.MoviesApiBase
	{
		public override Task<responseModel> GetAll(Empty request, ServerCallContext context)
		{
			var response = new responseModel
			{
				//success = true
				// ... populate other fields as necessary
			};

			return Task.FromResult(response);
		}
	}
}
