using Service.Data;
using Service.Data.Abstract;
using Service.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddScoped<IMessageDAL>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MessageDb");

    // Pass the connection string to the constructor
    return new MessageDAL(connectionString);
});


var app = builder.Build();

app.UseRouting();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<MessageHub>("/messageHub");
app.MapControllers();
app.UseHttpsRedirection();


app.Run();