var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<StructuredLogSender>();
var host = builder.Build();

var app = host.Services.GetRequiredService<StructuredLogSender>();

await app.RunAsync();