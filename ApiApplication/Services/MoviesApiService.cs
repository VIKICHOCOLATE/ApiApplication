using Grpc.Core;
using ProtoDefinitions;
using System.Threading.Tasks;

namespace ApiApplication.Services
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
