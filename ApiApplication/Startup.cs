using ApiApplication.Database;
using ApiApplication.Database.Repositories;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Features.Movies.Services;
using ApiApplication.Features.Seats.Services;
using ApiApplication.Features.ShowTimes.Services;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Mappings;
using ApiApplication.Shared.Middlewares;
using ApiApplication.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;

namespace ApiApplication
{
    public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<IShowtimesRepository, ShowtimesRepository>();
			services.AddTransient<ITicketsRepository, TicketsRepository>();
			services.AddTransient<IAuditoriumsRepository, AuditoriumsRepository>();

			services.AddTransient<IShowtimesService, ShowtimesService>();

			// For HTTP:
			services.AddTransient<IMovieService, ExternalMovieService>();

			// For gRPC:
			// There should be a Strategy pattern here to determine which service to use.
			// Unfortunately, I don't have enough time to implement it.
			// services.AddTransient<IMovieService, MoviesGrpcService>();

			services.AddTransient<ISeatsService, SeatsService>();
			services.AddSingleton<ICacheService, RedisCacheService>();

			services.AddHttpClient("ExternalMovies", config =>
			{
				config.BaseAddress = new Uri(Configuration["Services:ExternalMovies:http"]);
			}).AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(500)));

			services.AddDbContext<CinemaContext>(options =>
			{
				options.UseInMemoryDatabase("CinemaDb")
					.EnableSensitiveDataLogging()
					.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
			});

			services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = Configuration.GetConnectionString("RedisConnection");
			});

			services.AddAutoMapper(typeof(AutoMapping));

			services.AddControllers();

			services.AddHttpClient();
			services.AddGrpc();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<ExecutionTimeLoggingMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<MoviesGrpcService>();
				endpoints.MapControllers();
			});

			SampleData.Initialize(app);
		}
	}
}
