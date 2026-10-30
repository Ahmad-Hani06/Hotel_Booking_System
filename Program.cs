using BussinessLayerLogic.DI;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string not found.");

builder.Services.AddBusinessLayer(connectionString);

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddOpenApi();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    var urls = builder.Configuration["urls"]?.Split(";")?.FirstOrDefault() ?? "https://localhost:5001";
    var swaggerUrl = $"{urls.TrimEnd('/')}/swagger";

    try
    {

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = swaggerUrl,
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        //Console.WriteLine($"فشل فتح المتصفح: {ex.Message}");
    }
}

app.Run();