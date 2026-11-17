
using Microsoft.EntityFrameworkCore;
using EmployeeProjectAllocation.Backend.Data;
using EmployeeProjectAllocation.Backend.Middleware;
using EmployeeProjectAllocation.Backend.Services.Implementations;
using EmployeeProjectAllocation.Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Register CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Register Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Register Application Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAllocationService, AllocationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IProjectSkillService, ProjectSkillService>();
builder.Services.AddScoped<IResourcePoolService, ResourcePoolService>();
builder.Services.AddScoped<IProjectCandidateService, ProjectCandidateService>();
builder.Services.AddScoped<IProjectHiringSummaryService, ProjectHiringSummaryService>();

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
