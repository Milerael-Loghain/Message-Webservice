using Service.Data;
using Service.Data.Abstract;
using Service.Services;
using Service.Services.Abstract;
using Service.Utility;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var fileLoggerPath = $"logs/app-log-{DateTime.Now:yyyyMMdd}.log";
            builder.Services.AddSingleton<ILoggerProvider>(new FileLoggerProvider(fileLoggerPath));

            builder.Services.AddControllers();

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                // Path to the XML documentation file
                var xmlFile = Path.Combine(AppContext.BaseDirectory, "Service.xml");
                options.IncludeXmlComments(xmlFile); // Make sure to include this line
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            builder.Services.AddScoped<IMessageDAL>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                return new MessageDAL(connectionString);
            });

            builder.Services.AddSingleton<IWebSocketService, WebSocketService>();


            var app = builder.Build();

            app.UseWebSockets();

            app.UseRouting();

            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.Run();
        }
    }
}