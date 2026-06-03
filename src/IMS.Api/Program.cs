using System;
using FluentValidation;
using IMS.Application.Products.Command;
using IMS.Api.Middleware;
using IMS.Application.Products.Query;
using IMS.Application.Shared;
using IMS.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Mediator, FluentValidation, AutoMapper
builder.Services.AddValidatorsFromAssemblyContaining<CreateProduct.Validator>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<GetProducts>();
    cfg.AddOpenBehavior(typeof(PipelineValidationBehavior<,>));
});

builder.Services.AddAutoMapper(cfg =>
{
}, typeof(MappingProfiles));

// Middleware registrations
builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// Middleware usings
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

// Apply pending migrations and seed data
try
{
    var context = services.GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error during migration");
    throw;
}

app.Run();
