using MentorConnect.Web.BackgroundServices;
using MentorConnect.Web.Configurations;
using MentorConnect.Web.Filters;
using MentorConnect.Web.Hubs;
using MentorConnect.Web.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerTimeNotifier>();

builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Logging.AddLogsFilter();

builder.Services.AddCors();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCustomMiddleware(app.Environment);

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notifications");

app.Run();