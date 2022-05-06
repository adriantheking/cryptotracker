using Common.Connectors;
using Common.Connectors.Interfaces;
using Common.Options;
using Common.Services;
using Common.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddControllersWithViews();
//connectors
builder.Services.AddSingleton<IZonda, Zonda>();
//services
builder.Services.AddTransient<ITransactionService, TransactionService>();
//options 
builder.Services.Configure<ZondaConnectorOptions>(builder.Configuration.GetSection(ZondaConnectorOptions.SectionName));
var app = builder.Build();

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
