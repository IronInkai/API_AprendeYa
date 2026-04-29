using API_AprendeYa.Services;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// 👇 ESTA LÍNEA SOLUCIONA TODO EL MAPEO DE DAPPER
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// REGISTRO DE SERVICIOS
builder.Services.AddScoped<IForoService, ForoService>();
builder.Services.AddControllers();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<INivelService, NivelService>();
builder.Services.AddScoped<IModuloService, ModuloService>();
builder.Services.AddScoped<ITemaService, TemaService>();
builder.Services.AddScoped<IContenidoService, ContenidoService>();
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

// ==========================================
// CONFIGURACIÓN CORS (Permitir que el Frontend nos hable via JS)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin()    // Permite peticiones de cualquier puerto/dominio
              .AllowAnyHeader()    // Permite cualquier tipo de encabezado
              .AllowAnyMethod();   // Permite GET, POST, PUT, DELETE, etc.
    });
});

// 2. CONFIGURAR LA AUTENTICACIÓN JWT (¡TODO ANTES DEL BUILD!)
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
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<IVentaService, VentaService>();
// =========================================================
// ¡AQUÍ CERRAMOS LA MOCHILA Y CONSTRUIMOS LA APP! (Una sola vez)
// =========================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. EL ORDEN ES VITAL AQUÍ:
// Primero abrimos la puerta para los navegadores (CORS)
app.UseCors("PermitirFrontend");

// Segundo pedimos el Token (Authentication)
app.UseAuthentication();

// Tercero verificamos a dónde puede entrar (Authorization)
app.UseAuthorization();

app.MapControllers();

app.Run();