using EmployeeProjectAllocation.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAllocation> ProjectAllocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeCode);

            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeCode)
                .HasMaxLength(8);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // EmployeeSkill: Composite Primary Key
            modelBuilder.Entity<EmployeeSkill>()
                .HasKey(es => new { es.EmployeeCode, es.SkillId });

            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.EmployeeSkills)
                .HasForeignKey(es => es.EmployeeCode);

            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Skill)
                .WithMany(s => s.EmployeeSkills)
                .HasForeignKey(es => es.SkillId);

            // Project
            modelBuilder.Entity<Project>()
                .HasKey(p => p.ProjectId);

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectCode)
                .IsUnique();

            // ProjectAllocation
            modelBuilder.Entity<ProjectAllocation>()
                .HasKey(a => a.AllocationId);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeCode);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(a => a.Project)
                .WithMany()
                .HasForeignKey(a => a.ProjectId);
        }
    }
}