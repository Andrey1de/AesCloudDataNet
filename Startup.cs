using AesCloudDataNet.DB;
using AesCloudDataNet.Services;
using AesCloudDataNet.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json;

namespace AasCloudData
{
    public class Startup
    {
        // readonly string DATABASE_URL = "";
        readonly string ConnectionString = "";
        readonly bool IS_HEROKU;
         string DATABASE_URL;

        bool isStr(string str) => str != null && str.Length > 10;
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            IS_HEROKU = bool.TryParse(Environment.GetEnvironmentVariable("IS_HEROKU"), out IS_HEROKU);
            DATABASE_URL = Environment.GetEnvironmentVariable("DATABASE_URL");
       // mysql://b1b11a6c18e438:97035749@us-cdbr-east-03.cleardb.com/heroku_16d06713d5b8727?reconnect=true
            if (IS_HEROKU)
            {
                Console.WriteLine("IS_HEROKU:" + IS_HEROKU.ToString().ToUpper());

                if (!string.IsNullOrEmpty(DATABASE_URL))
                {
                    ConnectionString = DBUrlParse.ParseDatabaseUrl(DATABASE_URL);
                }
            }
            else
            {
                ConnectionString = Configuration.GetConnectionString("MySqlCinnectionString");

            }
            Console.WriteLine("ConnectionString:" + ConnectionString);

            //if (IS_HEROKU)
            //{

            //    DATABASE_URL = Environment.GetEnvironmentVariable("DATABASE_URL");// ?? DATABASE_URL_MOK;
            //    if (isStr(DATABASE_URL))
            //    {
            //        PostgresConnectionString = ForPostgress.ParseDatabaseUrl(DATABASE_URL);
            //        if (isStr(PostgresConnectionString))
            //        {
            //            Console.WriteLine("PostgresConnectionString:" + PostgresConnectionString);
            //            Environment.SetEnvironmentVariable("POSTGRES_CONNECTION_STRING", PostgresConnectionString);
            //        }
            //    }
            //}
            //else
            //{
            //    PostgresConnectionString = Configuration.GetConnectionString("PostgresConnectionString");
            //    Console.WriteLine("PostgresConnectionString:" + PostgresConnectionString);

            //}

           // ConnectionString = Configuration.GetConnectionString("MySqlCinnectionString");
            Console.ForegroundColor = old;

        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
         {
             builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
         }));
            services.AddHttpClient();


            //        if (isStr(PostgresConnectionString))
            //      {
            _ = services.AddDbContext<AesCloudDataContext>(options =>
            {
                options.UseMySQL(ConnectionString);
                Console.WriteLine($"AddDbContext:UseNpgsql({ConnectionString})");
            });
            //  }

            services.AddScoped<IRateToUsdService, RateToUsdService>();

            services.AddScoped<IUserService, UserService>();


            Console.ForegroundColor = old;

            services.AddControllers().
                AddJsonOptions(option =>
                option.JsonSerializerOptions.PropertyNamingPolicy
                = JsonNamingPolicy.CamelCase
                 ); ;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AasCloudData", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("MyPolicy");

            if (true || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AasCloudData v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
