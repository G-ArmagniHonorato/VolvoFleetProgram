using VolvoFleetProgram.Models.Repositories;
using VolvoFleetProgram.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });



builder.Services.AddSingleton<IVehicleRepository, CacheVehicleRepository>();
builder.Services.AddSingleton<IVehicleService, VehicleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
