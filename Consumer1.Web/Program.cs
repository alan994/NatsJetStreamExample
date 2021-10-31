using Consumer1.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ConsumerA>();
builder.Services.AddHostedService<ConsumerB>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
