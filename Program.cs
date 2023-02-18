using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Claims;
using System.Net;
using Microsoft.OpenApi.Models;
using System.Reflection;

var  allowSpecificOrigins = "_allowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
                      policy  =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header. \r\nExample: 'Bearer mytoken'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Authorization"
        },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "立交桥 Server API",
        Description = "An ASP.NET Core Web API for 立交桥",
        Contact = new OpenApiContact
        {
            Name = "Github",
            Url = new Uri("https://github.com/Green-Hair/Server")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://github.com/Green-Hair/Server/blob/main/LICENSE")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddDbContext<ServerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ServerContextSQLite")));
builder.Services.AddDbContext<SecurityContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SecurityContextSQLite")));
    
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<SecurityContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/auth/login"))
                .AddJwtBearer(opts => {
                    opts.RequireHttpsMetadata = false;
                    opts.SaveToken = true;
                    opts.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(builder.Configuration["jwtSecret"])
                        ),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                    };
                    opts.Events = new JwtBearerEvents {
                        OnTokenValidated = async ctx => {
                            var usrmgr = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                            var signinmgr = ctx.HttpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
                            string? username = ctx.Principal?.FindFirst(ClaimTypes.Name)?.Value;
                            var idUser = await usrmgr.FindByNameAsync(username);
                            ctx.Principal = await signinmgr.CreateUserPrincipalAsync(idUser);
                        },
                    };
                });

builder.Services.Configure<IdentityOptions>(opts => {
    opts.Password.RequireLowercase = false;
    opts.Password.RequiredLength = 6;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireDigit = false;
    opts.User.RequireUniqueEmail = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API V1");
    c.RoutePrefix = string.Empty;
});
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ServerContext>();
    context.Database.EnsureCreated();
    var securityContext = services.GetRequiredService<SecurityContext>();
    securityContext.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.UseCors(allowSpecificOrigins);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
