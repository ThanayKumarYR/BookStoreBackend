using BusinessLayer.Interface;
using BusinessLayer.Service;
using Repository.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// NLog
var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
builder.Logging.AddDebug();
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IUserRepositoryLayer, UserServiceRepositoryLayer>();
builder.Services.AddScoped<IUserBusinessLayer, UserServiceBusinessLayer>();
builder.Services.AddScoped<IUserAddressRepositoryLayer, UserAddressServiceRepositoryLayer>();
builder.Services.AddScoped<IUserAddressBusinessLayer,UserAddressServiceBusinessLayer>();
builder.Services.AddScoped<IBookRepositoryLayer,BookServiceRepositoryLayer>();
builder.Services.AddScoped<IBookBusinessLayer, BookServiceBusinessLayer>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();