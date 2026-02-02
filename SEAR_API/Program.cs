using SEAR_DataContract.Misc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
    app.UseExceptionHandler("/ApiHome/Error");
    app.UseHsts();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
                      .GetRequiredService<ILoggerFactory>()
                      .CreateLogger("SEAR API");

    AppLogger.Initialize(logger);
}

app.Run();