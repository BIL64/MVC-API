using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Lms.Data.Data;
using Lms.Api.Extensions;
using Lms.Core.Repositories;
using Lms.Data.Data.Repositories;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LmsApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LmsApiContext") ?? throw new InvalidOperationException("Connection string 'LmsApiContext' not found.")));

// Add services to the container.
builder.Services.AddScoped<IUoW, UoW>();

builder.Services.AddControllers(opt => opt.ReturnHttpNotAcceptable = true)    
    .AddNewtonsoftJson(setupAction =>
    {
        setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }).AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(LmsMappings));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.SeedDataAsync().GetAwaiter().GetResult();
app.Run();
