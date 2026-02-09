using System.Text;
using HRManagement.Api.Services;
using HRManagement.Application.Interfaces;
using HRManagement.Application.Services;
using HRManagement.Infrastructure;
using HRManagement.Infrastructure.Identity;
using HRManagement.Infrastructure.Services;
using HRManagement.Infrastructure.Persistence;
using HRManagement.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore;
using HRManagement.Api;
using HRManagement.Api.Middleware;
using HRManagement.Application.Mapping;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing in appsettings.json.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ------------------------------------------------------------------
// 2. AUTHENTICATION & JWT SETUP
// ------------------------------------------------------------------
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    var secret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret is missing in appsettings.json.");
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});


builder.Services.AddInfrastructure(); 

builder.Services.AddScoped<ICourseTypeService, CourseTypeService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEmployeeCourseService, EmployeeCourseService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// The "Multi-Tenancy Bridge"
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddAutoMapper(typeof(CourseMappingProfile).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddJwtSecurity());

var app = builder.Build();

await SeedRolesAsync(app);
await SeedDataAsync(app);

// ------------------------------------------------------------------
// 5. MIDDLEWARE PIPELINE
// ------------------------------------------------------------------
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedRolesAsync(WebApplication webApp)
{
    using var scope = webApp.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in new[] { "Admin", "HRUser" })
    {
        if (await roleManager.RoleExistsAsync(roleName)) continue;
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

static async Task SeedDataAsync(WebApplication webApp)
{
    using var scope = webApp.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DataSeeder.SeedAsync(db);
}