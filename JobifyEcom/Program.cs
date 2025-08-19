using JobifyEcom.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Services;
using JobifyEcom.Helpers;
using System.Text.Json;
using System.Reflection;
using Microsoft.OpenApi.Models;
using JobifyEcom.DTOs;
using JobifyEcom.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using JobifyEcom.Security;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//--------------- Global JsonSerializerOptions ---------------
JsonSerializerOptions globalJsonOptions = JsonOptionsFactory.Create();

// Register as singleton so it's available everywhere
builder.Services.AddSingleton(globalJsonOptions);

//--------------- Database connection ---------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

//--------------- Controllers use same JSON options ---------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = globalJsonOptions.PropertyNamingPolicy;
        options.JsonSerializerOptions.DefaultIgnoreCondition = globalJsonOptions.DefaultIgnoreCondition;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = globalJsonOptions.PropertyNameCaseInsensitive;
        options.JsonSerializerOptions.ReferenceHandler = globalJsonOptions.ReferenceHandler;

        foreach (JsonConverter converter in globalJsonOptions.Converters)
        {
            options.JsonSerializerOptions.Converters.Add(converter);
        }
    }
);

//--------------- JWT Authentication ---------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");

        string secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("Missing JWT configuration: 'SecretKey' value is not set in JwtSettings.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        // Use the global JSON options for JWT events
        options.Events = JwtEventHandlers.Create(globalJsonOptions);
    }
);

//--------------- Services & Auth ---------------
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

//--------------- Model validation responses ---------------
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        List<string> errors = [.. context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage)];

        var response = ApiResponse<object>.Fail(null, "Some of the provided data is invalid.", errors);
        return new BadRequestObjectResult(response);
    };
});

//--------------- Swagger (only in dev) ---------------
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        string apiDescription = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "OpenApiDescription.md"));

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "JobifyEcom",
            Version = "1.0",
            Description = apiDescription,
        });

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath, true);

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter the access token you are given after you login.",
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {{
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }});
    });
}

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });

    app.MapScalarApiReference(options =>
    {
        options.Title = "JobifyEcom API";
        options.AddServer("http://localhost:5122", "Development");
        options.AddServer("https://localhost:5122", "Production");
        options.DefaultHttpClient = new(ScalarTarget.Node, ScalarClient.Fetch);
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
