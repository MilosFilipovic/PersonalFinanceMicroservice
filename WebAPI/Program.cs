using Application.Common.Behaviors;
using Application.Common.Models;
using Application.Features.Transactions.Commands.CreateTransaction;
using Application.Features.Transactions.Commands.SplitTransaction;
using Application.Mappings;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using WebAPI.Extensions;
using YamlDotNet.Serialization;



var builder = WebApplication.CreateBuilder(args);


var yamlPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "rules.yaml"
);
var yaml = File.ReadAllText(yamlPath);
var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
var yamlObj = deserializer.Deserialize<Dictionary<string, List<CategorizationRule>>>(yaml);
var rules = yamlObj["Rules"];
builder.Services.Configure<List<CategorizationRule>>(opts => {
    opts.Clear();
    opts.AddRange(rules);
});



// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PersonalFinance API", Version = "v1" });
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

// DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//    ));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection")
    ));


// Kontroleri
builder.Services.AddControllers();

// IRequestHandler<> klase iz Application
builder.Services.AddMediatR(typeof(CreateTransactionCommandHandler).Assembly);

// AutoMapper
builder.Services.AddAutoMapper(typeof(TransactionMappingProfile).Assembly);


// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionCommandValidator>();

// ValidationBehavior u MediatR pipeline da svaka komanda/query prolazi validaciju
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);
//builder.Services.AddTransient<IValidator<SplitTransactionCommand>, SplitTransactionCommandValidator>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionCsvParser, TransactionCsvParser>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryCsvParser, CategoryCsvParser>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionSplitRepository, TransactionSplitRepository>();




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseDefaultFiles();
app.UseStaticFiles();     


app.MapControllers();

app.UseApiExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();



