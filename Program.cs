using API_AprendeYa.Services;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer; // NUEVO
using Microsoft.IdentityModel.Tokens; // NUEVO
using Microsoft.OpenApi.Models; // NUEVO
using System.Text; // NUEVO

var builder = WebApplication.CreateBuilder(args);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddEndpointsApiExplorer();

// 1. CONFIGURAR SWAGGER PARA QUE ACEPTE TOKENS
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_AprendeYa", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa 'Bearer ' seguido de tu token. Ejemplo: Bearer eyJhbGci...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer"}
        },
        new string[]{}
    }});
});

// 2. CONFIGURAR LA AUTENTICACIÓN JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<ICursoService, CursoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. ¡IMPORTANTE! UseAuthentication debe ir ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();