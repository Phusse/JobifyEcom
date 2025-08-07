using JobifyEcom.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JobifyEcom.Services;
using JobifyEcom.Helpers;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using Microsoft.OpenApi.Models;
using JobifyEcom.DTOs;
using JobifyEcom.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//--------------- Add database connection ---------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

//--------------- JSON Options ---------------
builder.Services.Configure<JsonOptions>(options =>
{
    JsonSerializerOptions serializerOptions = options.JsonSerializerOptions;
    serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    serializerOptions.PropertyNameCaseInsensitive = true;
    serializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    serializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
    );
});

//--------------- Register JwtEventHandlers ---------------
builder.Services.AddSingleton(provider =>
{
    JsonSerializerOptions jsonOptions = provider.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
    return JwtEventHandlers.Create(jsonOptions);
});

//--------------- JWT Authentication ---------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IServiceProvider>((options, provider) =>
    {
        IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
        string secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        var jwtEvents = provider.GetRequiredService<JwtBearerEvents>();

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

        options.Events = jwtEvents;
    }
);

//--------------- Register application services ---------------
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

//--------------- Configure model validation responses ---------------
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        List<string> errors = [.. context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage)
        ];

        var response = ApiResponse<object?>.Fail(null, "Invalid input data", errors);
        return new BadRequestObjectResult(response);
    };
});

//--------------- Swagger (only in dev) ---------------
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
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
            Description = "Enter the token you are given after you login.\n\nExample: \"token eyJhbGciOiJIUzI1NiIsInR5cCI6...\"",
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

//--------------- Build & run the app ---------------
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
