using MentorConnect.Web.Configurations;
using MentorConnect.Web.Filters;
using MentorConnect.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Logging.AddLogsFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCustomMiddleware(app.Environment);

app.Run();