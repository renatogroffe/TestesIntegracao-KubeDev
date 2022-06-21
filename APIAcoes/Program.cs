using FluentValidation;
using FluentValidation.AspNetCore;
using StackExchange.Redis;
using APIAcoes.Models;
using APIAcoes.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddFluentValidation();

builder.Services.AddTransient<IValidator<Acao>, AcaoValidator>();

builder.Services.AddSingleton<ConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("RedisServer")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
