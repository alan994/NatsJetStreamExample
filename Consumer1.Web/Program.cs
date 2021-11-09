using Consumer1.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ConsumerSyncA>();
builder.Services.AddHostedService<ConsumerSyncB>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
