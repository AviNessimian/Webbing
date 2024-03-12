using Webbing.Assignment.Api.JsonConverters;

const string ORIGIN_POLICY = "MW2.0_CLIENT_ORIGIN_POLICY";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Install the service
builder.Services.AddAppService();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: ORIGIN_POLICY,
        policy =>
        {
            policy
                .WithOrigins(
                    builder.Environment.IsDevelopment() ?
                    new string[] { "http://localhost:4200", "http:/localhost:49983", "https://localhost:44451" }
                    : Array.Empty<string>()
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(ORIGIN_POLICY);

app.MapControllers();

app.Run();
