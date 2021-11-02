using Consumer1.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ConsumerSyncB>();
builder.Services.AddHostedService<ConsumerSyncA>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
