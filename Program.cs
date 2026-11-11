
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.Services.Interfaces;
using EmployeeProjectAllocation.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using EmployeeProjectAllocation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
