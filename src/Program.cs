using GruenesBrett.Initialization;

var builder = WebApplication.CreateBuilder(args);

Services.Configure(builder);

var app = builder.Build();

await App.ConfigureAsync(app);

app.Run();
