import {
  Component,
  OnInit,
  ChangeDetectorRef,
  HostListener
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ResourcePoolService } from '../../core/services/resource-pool.service';
import { ProjectService } from '../../core/services/project.service';

import { ResourcePoolEmployee } from '../../core/models/resource-pool.model';
import { ProjectResponse } from '../../core/models/project.model';

@Component({
  selector: 'app-resource-pool',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './resource-pool.html',
  styleUrl: './resource-pool.scss'
})
export class ResourcePool implements OnInit {

  /** Number of skill tags shown in the table before collapsing into "+N" */
  private readonly maxVisibleSkills = 4;

  employees: ResourcePoolEmployee[] = [];
  projects: ProjectResponse[] = [];

  loading = false;
  loadingProjects = false;

  errorMessage = '';

  searchTerm = '';
  selectedProjectId: number | null = null;
  selectedLocation = '';
  selectedSkill = '';

  selectedEmployee: ResourcePoolEmployee | null = null;

  constructor(
    private resourcePoolService: ResourcePoolService,
    private projectService: ProjectService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProjects();
    this.loadEmployees();
  }

  // CLOSE MODAL WITH ESCAPE KEY
  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.selectedEmployee) {
      this.closeEmployeeDetails();
    }
  }

  // LOAD PROJECTS
  loadProjects(): void {
    this.loadingProjects = true;

    this.projectService.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.loadingProjects = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Unable to load projects:', error);
        this.loadingProjects = false;
        this.cdr.detectChanges();
      }
    });
  }

  // LOAD AVAILABLE EMPLOYEES
  loadEmployees(projectId?: number | null): void {
    this.loading = true;
    this.errorMessage = '';

    this.resourcePoolService
      .getAvailableEmployees(projectId)
      .subscribe({
        next: (employees) => {
          this.employees = employees;
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Unable to load resource pool:', error);

          this.errorMessage =
            error?.error?.message ||
            'Unable to load available employees.';

          this.loading = false;
          this.cdr.detectChanges();
        }
      });
  }

  // PROJECT FILTER
  onProjectChange(): void {
    this.selectedEmployee = null;
    this.selectedLocation = '';
    this.selectedSkill = '';

    this.loadEmployees(this.selectedProjectId);
  }

  // GET UNIQUE LOCATIONS
  get locations(): string[] {
    return [
      ...new Set(
        this.employees
          .map(employee => employee.location)
          .filter((location): location is string => !!location)
      )
    ].sort();
  }

  // GET UNIQUE SKILLS
  get skills(): string[] {
    return [
      ...new Set(
        this.employees.flatMap(employee => employee.skills || [])
      )
    ].sort();
  }

  // FILTER EMPLOYEES
  get filteredEmployees(): ResourcePoolEmployee[] {
    const search = this.searchTerm.trim().toLowerCase();
    const skill = this.selectedSkill.toLowerCase();

    return this.employees.filter(employee => {

      const matchesSearch =
        !search ||
        employee.employeeCode.toLowerCase().includes(search) ||
        employee.fullName.toLowerCase().includes(search) ||
        employee.email.toLowerCase().includes(search);

      const matchesLocation =
        !this.selectedLocation ||
        employee.location === this.selectedLocation;

      const matchesSkill =
        !skill ||
        !!employee.skills?.some(s => s.toLowerCase() === skill);

      return matchesSearch && matchesLocation && matchesSkill;
    });
  }

  // CLEAR ALL FILTERS (reloads only if a project filter was active)
  clearFilters(): void {
    const hadProject = this.selectedProjectId !== null;

    this.searchTerm = '';
    this.selectedLocation = '';
    this.selectedSkill = '';
    this.selectedProjectId = null;
    this.selectedEmployee = null;

    if (hadProject) {
      this.loadEmployees();
    }
  }

  // OPEN EMPLOYEE DETAILS
  viewEmployee(employee: ResourcePoolEmployee): void {
    this.selectedEmployee = employee;
  }

  // CLOSE EMPLOYEE DETAILS
  closeEmployeeDetails(): void {
    this.selectedEmployee = null;
  }

  // REFRESH RESOURCE POOL
  refresh(): void {
    this.selectedEmployee = null;
    this.loadEmployees(this.selectedProjectId);
  }

  // SKILL TAG HELPERS
  visibleSkills(employee: ResourcePoolEmployee): string[] {
    return (employee.skills || []).slice(0, this.maxVisibleSkills);
  }

  hiddenSkillCount(employee: ResourcePoolEmployee): number {
    return Math.max((employee.skills?.length || 0) - this.maxVisibleSkills, 0);
  }

  hiddenSkills(employee: ResourcePoolEmployee): string {
    return (employee.skills || []).slice(this.maxVisibleSkills).join(', ');
  }

  // TRACK BY FUNCTIONS
  trackByEmployee(_: number, employee: ResourcePoolEmployee): string {
    return employee.employeeCode;
  }

  trackByProject(_: number, project: ProjectResponse): number {
    return project.projectId;
  }

  // TOTAL AVAILABLE EMPLOYEES
  get totalEmployees(): number {
    return this.employees.length;
  }

  // TOTAL UNIQUE SKILLS
  get totalSkills(): number {
    return this.skills.length;
  }

  // TOTAL EMPLOYEE LOCATIONS
  get totalLocations(): number {
    return this.locations.length;
  }

  // CHECK IF FILTERS ARE ACTIVE
  get hasActiveFilters(): boolean {
    return !!(
      this.searchTerm ||
      this.selectedLocation ||
      this.selectedSkill ||
      this.selectedProjectId !== null
    );
  }
}
