using Binance.Common;
using CryptoCommon.Connectors;
using CryptoCommon.Connectors.Interfaces;
using CryptoCommon.Options;
using CryptoCommon.Services;
using CryptoCommon.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddControllersWithViews();
//connectors
builder.Services.AddSingleton<IZonda, Zonda>();
builder.Services.AddSingleton<IBinance, BinanceConnector>();
//services
builder.Services.AddTransient<IZondaService, ZondaService>();
builder.Services.AddTransient<IBinanceService, CryptoCommon.Services.BinanceService>();
//httpclients
builder.Services.AddHttpClient<IBinance, BinanceConnector>();

//options 
builder.Services.Configure<ZondaConnectorOptions>(builder.Configuration.GetSection(ZondaConnectorOptions.SectionName));
builder.Services.Configure<BinanceConnectorOptions>(builder.Configuration.GetSection(BinanceConnectorOptions.SectionName));
//other
builder.Services.AddLazyCache();
//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy => {
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader(); 
    });
});
builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
var app = builder.Build();


//cors
app.UseCors("default");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
