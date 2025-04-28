using Johnson.API;
using Johnson.API.Services;
using Johnson.Infra.DataStorage;
using Johnson.Infra.EventBus;
using Johnson.Infra.RegistryCache;
using Johnson.Monitoring;

var builder = WebApplication.CreateBuilder(args);

var apiVersion = builder.Configuration["APIVersion"]
    ?? throw new Exception("Cannot find API Version");

builder.Services.AddControllers(options =>
    options.Conventions.Add(new RoutePrefixConvention("api", apiVersion)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRabbitMQ(builder.Configuration);

builder.Services.AddAPIServices(builder.Configuration);
builder.Services.AddMonitoringTools();

//builder.Services.AddHostedService<ServiceLoader>();

//builder.Services.AddPostRoutingMiddlewares();
//builder.Services.AddPreRoutingMiddlewares();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
