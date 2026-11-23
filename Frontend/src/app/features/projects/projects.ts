
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  FormGroup
} from '@angular/forms';

import { ProjectService } from '../../core/services/project.service';

import {
  ProjectResponse,
  CreateProject,
  UpdateProject
} from '../../core/models/project.model';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './projects.html',
  styleUrl: './projects.scss'
})
export class Projects implements OnInit {

  // Data
  projects: ProjectResponse[] = [];

  // Loading and submission
  loading = false;
  submitting = false;
  deletingProject = false;

  // Modal visibility
  showForm = false;
  showDeleteModal = false;

  // Form mode
  isEditMode = false;
  editingProjectId: number | null = null;

  // Selected project
  selectedProject: ProjectResponse | null = null;

  // Messages
  errorMessage = '';
  formErrorMessage = '';
  deleteErrorMessage = '';
  successMessage = '';

  // Search and filter
  searchTerm = '';
  statusFilter = 'All';

  // Pagination
  currentPage = 1;
  pageSize = 5;

  // Reactive form
  projectForm!: FormGroup;

  constructor(
    private projectService: ProjectService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.projectForm = this.fb.group({

      projectCode: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(20)
        ]
      ],

      projectName: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(150)
        ]
      ],

      clientName: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(100)
        ]
      ],

      technology: [
        '',
        Validators.maxLength(100)
      ],

      requiredEmployees: [
        1,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],

      location: [
        '',
        Validators.maxLength(100)
      ],

      startDate: [''],

      endDate: [''],

      status: [
        'Active',
        Validators.required
      ]

    });
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  // ==========================================
  // GET ALL PROJECTS
  // ==========================================

  loadProjects(): void {

    this.loading = true;
    this.errorMessage = '';

    this.projectService.getProjects().subscribe({

      next: (response) => {

        this.projects = response;

        this.loading = false;

        this.currentPage = Math.min(
          this.currentPage,
          this.totalPages
        );

        this.cdr.markForCheck();
      },

      error: (error) => {

        console.error('Error loading projects:', error);

        this.errorMessage =
          'Unable to load projects. Please try again.';

        this.loading = false;

        this.cdr.markForCheck();
      }

    });
  }

  // ==========================================
  // SEARCH AND FILTER
  // ==========================================

  get filteredProjects(): ProjectResponse[] {

    const search = this.searchTerm.trim().toLowerCase();

    return this.projects.filter(project => {

      const matchesSearch =
        !search ||
        project.projectCode.toLowerCase().includes(search) ||
        project.projectName.toLowerCase().includes(search) ||
        project.clientName.toLowerCase().includes(search);

      const matchesStatus =
        this.statusFilter === 'All' ||
        project.status === this.statusFilter;

      return matchesSearch && matchesStatus;

    });
  }

  onSearchChange(event: Event): void {

    const target = event.target as HTMLInputElement;

    this.searchTerm = target.value;

    this.currentPage = 1;
  }

  onStatusFilterChange(event: Event): void {

    const target = event.target as HTMLSelectElement;

    this.statusFilter = target.value;

    this.currentPage = 1;
  }

  clearFilters(): void {

    this.searchTerm = '';
    this.statusFilter = 'All';
    this.currentPage = 1;
  }

  // ==========================================
  // PAGINATION
  // ==========================================

  get totalPages(): number {

    return Math.max(
      1,
      Math.ceil(this.filteredProjects.length / this.pageSize)
    );
  }

  get paginatedProjects(): ProjectResponse[] {

    const startIndex =
      (this.currentPage - 1) * this.pageSize;

    return this.filteredProjects.slice(
      startIndex,
      startIndex + this.pageSize
    );
  }

  get startItem(): number {

    return this.filteredProjects.length === 0
      ? 0
      : (this.currentPage - 1) * this.pageSize + 1;
  }

  get endItem(): number {

    return Math.min(
      this.currentPage * this.pageSize,
      this.filteredProjects.length
    );
  }

  goToPage(page: number): void {

    if (page < 1 || page > this.totalPages) {
      return;
    }

    this.currentPage = page;
  }

  previousPage(): void {

    this.goToPage(this.currentPage - 1);
  }

  nextPage(): void {

    this.goToPage(this.currentPage + 1);
  }

  // ==========================================
  // FORM RESET
  // ==========================================

  resetForm(): void {

    this.projectForm.reset({

      projectCode: '',
      projectName: '',
      clientName: '',
      technology: '',
      requiredEmployees: 1,
      location: '',
      startDate: '',
      endDate: '',
      status: 'Active'

    });

    this.projectForm.markAsPristine();
    this.projectForm.markAsUntouched();

    this.formErrorMessage = '';
  }

  // ==========================================
  // OPEN CREATE FORM
  // ==========================================

  openCreateForm(): void {

    this.editingProjectId = null;
    this.isEditMode = false;

    this.resetForm();

    this.successMessage = '';
    this.showForm = true;
  }

  // ==========================================
  // OPEN EDIT FORM
  // ==========================================

  openEditForm(project: ProjectResponse): void {

    this.editingProjectId = project.projectId;
    this.isEditMode = true;

    this.resetForm();

    this.projectForm.patchValue({

      projectCode: project.projectCode,
      projectName: project.projectName,
      clientName: project.clientName,
      technology: project.technology || '',
      requiredEmployees: project.requiredEmployees,
      location: project.location || '',
      startDate: project.startDate?.substring(0, 10) || '',
      endDate: project.endDate?.substring(0, 10) || '',
      status: project.status

    });

    this.formErrorMessage = '';
    this.successMessage = '';

    this.showForm = true;
  }

  // ==========================================
  // CLOSE FORM
  // ==========================================

  closeCreateForm(): void {

    if (this.submitting) {
      return;
    }

    this.showForm = false;

    this.editingProjectId = null;
    this.isEditMode = false;

    this.resetForm();
  }

  // ==========================================
  // DATE VALIDATION
  // ==========================================

  private isDateRangeValid(
    startDate: string | null,
    endDate: string | null
  ): boolean {

    if (!startDate || !endDate) {
      return true;
    }

    return new Date(endDate) >= new Date(startDate);
  }

  // ==========================================
  // CREATE PROJECT - POST
  // ==========================================

  createProject(): void {

    if (this.projectForm.invalid) {

      this.projectForm.markAllAsTouched();
      return;
    }

    const formValue = this.projectForm.getRawValue();

    const project: CreateProject = {

      projectCode: formValue.projectCode.trim(),

      projectName: formValue.projectName.trim(),

      clientName: formValue.clientName.trim(),

      technology: formValue.technology?.trim() || null,

      requiredEmployees: Number(
        formValue.requiredEmployees
      ),

      location: formValue.location?.trim() || null,

      startDate: formValue.startDate || null,

      endDate: formValue.endDate || null

    };

    if (!this.isDateRangeValid(
      project.startDate ?? null,
      project.endDate ?? null
    )) {

      this.formErrorMessage =
        'End date cannot be earlier than start date.';

      return;
    }

    this.submitting = true;
    this.formErrorMessage = '';

    this.projectService.createProject(project).subscribe({

      next: () => {

        this.submitting = false;

        this.showForm = false;

        this.resetForm();

        this.isEditMode = false;
        this.editingProjectId = null;

        this.successMessage =
          'Project created successfully.';

        this.currentPage = 1;

        this.loadProjects();

        this.cdr.markForCheck();
      },

      error: (error) => {

        console.error('Error creating project:', error);

        this.formErrorMessage =
          error.error?.message ||
          'Unable to create project. Please try again.';

        this.submitting = false;

        this.cdr.markForCheck();
      }

    });
  }

  // ==========================================
  // UPDATE PROJECT - PUT
  // ==========================================

  updateProject(): void {

    if (
      this.projectForm.invalid ||
      this.editingProjectId === null
    ) {

      this.projectForm.markAllAsTouched();
      return;
    }

    const formValue = this.projectForm.getRawValue();

    const project: UpdateProject = {

      projectName: formValue.projectName.trim(),

      clientName: formValue.clientName.trim(),

      technology: formValue.technology?.trim() || null,

      requiredEmployees: Number(
        formValue.requiredEmployees
      ),

      location: formValue.location?.trim() || null,

      startDate: formValue.startDate || null,

      endDate: formValue.endDate || null,

      status: formValue.status as UpdateProject['status']

    };

    if (!this.isDateRangeValid(
      project.startDate ?? null,
      project.endDate ?? null
    )) {

      this.formErrorMessage =
        'End date cannot be earlier than start date.';

      return;
    }

    this.submitting = true;
    this.formErrorMessage = '';

    this.projectService.updateProject(
      this.editingProjectId,
      project
    ).subscribe({

      next: () => {

        this.submitting = false;

        this.showForm = false;

        this.resetForm();

        this.isEditMode = false;
        this.editingProjectId = null;

        this.successMessage =
          'Project updated successfully.';

        this.loadProjects();

        this.cdr.markForCheck();
      },

      error: (error) => {

        console.error('Error updating project:', error);

        this.formErrorMessage =
          error.error?.message ||
          'Unable to update project. Please try again.';

        this.submitting = false;

        this.cdr.markForCheck();
      }

    });
  }

  // ==========================================
  // DELETE CONFIRMATION
  // ==========================================

  openDeleteConfirmation(project: ProjectResponse): void {

    this.selectedProject = project;

    this.deleteErrorMessage = '';
    this.successMessage = '';

    this.showDeleteModal = true;
  }

  // ==========================================
  // CANCEL DELETE
  // ==========================================

  cancelDelete(): void {

    if (this.deletingProject) {
      return;
    }

    this.showDeleteModal = false;

    this.selectedProject = null;

    this.deleteErrorMessage = '';
  }

  // ==========================================
  // DELETE PROJECT - DELETE
  // ==========================================

  confirmDelete(): void {

    if (
      !this.selectedProject ||
      this.deletingProject
    ) {
      return;
    }

    const projectId =
      this.selectedProject.projectId;

    this.deletingProject = true;
    this.deleteErrorMessage = '';

    this.projectService.deleteProject(projectId).subscribe({

      next: () => {

        this.deletingProject = false;

        this.showDeleteModal = false;

        this.selectedProject = null;

        this.successMessage =
          'Project deleted successfully.';

        this.loadProjects();

        this.cdr.markForCheck();
      },

      error: (error) => {

        console.error('Error deleting project:', error);

        this.deleteErrorMessage =
          error.error?.message ||
          'Unable to delete project. Please try again.';

        this.deletingProject = false;

        this.cdr.markForCheck();
      }

    });
  }

}
