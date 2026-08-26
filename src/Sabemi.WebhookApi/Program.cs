using Microsoft.EntityFrameworkCore;
using Sabemi.WebhookApi.BackgroundProcessing;
using Sabemi.WebhookApi.Data;
using Sabemi.WebhookApi.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SabemiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ApiKeyAuthFilter>();

// Fila em memoria + worker que consome ela em background (feat/background-worker).
builder.Services.AddSingleton<PagamentoProcessingQueue>();
builder.Services.AddHostedService<PagamentoBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
