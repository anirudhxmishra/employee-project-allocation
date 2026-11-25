
import { CommonModule } from '@angular/common';
import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ProjectService } from '../../core/services/project.service';
import { ResourcePoolService } from '../../core/services/resource-pool.service';
import { ProjectCandidateService } from '../../core/services/project-candidate.service';

import { ProjectResponse } from '../../core/models/project.model';
import { ResourcePoolEmployee } from '../../core/models/resource-pool.model';
import {
  ProjectCandidate,
  CandidateStatus,
  CreateProjectCandidate,
  UpdateCandidateStatus
} from '../../core/models/project-candidate.model';

type ModalType = 'add' | 'status' | 'details' | null;

@Component({
  selector: 'app-project-candidates',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './project-candidates.html',
  styleUrl: './project-candidates.scss'
})
export class ProjectCandidates implements OnInit {
  projects: ProjectResponse[] = [];
  candidates: ProjectCandidate[] = [];
  availableEmployees: ResourcePoolEmployee[] = [];

  selectedProjectId: number | null = null;

  searchTerm = '';
  statusFilter = 'All';

  loading = false;
  saving = false;
  errorMessage = '';
  successMessage = '';

  currentPage = 1;
  pageSize = 8;

  modalType: ModalType = null;
  selectedCandidate: ProjectCandidate | null = null;

  selectedEmployeeCode = '';
  addRemarks = '';

  editStatus: CandidateStatus = 'Selected';
  editRole = '';
  editInterviewDate = '';
  editRemarks = '';

  readonly statuses: CandidateStatus[] = [
    'Selected',
    'InterviewScheduled',
    'Rejected'
  ];
  readonly Math = Math;

  constructor(
    private projectService: ProjectService,
    private resourcePoolService: ResourcePoolService,
    private candidateService: ProjectCandidateService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;

        if (projects.length > 0) {
          this.selectedProjectId = projects[0].projectId;
          this.loadCandidates();
        } else {
          this.candidates = [];
        }

        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage = this.getErrorMessage(
          error,
          'Unable to load projects.'
        );
        this.cdr.detectChanges();
      }
    });
  }

  onProjectChange(): void {
    this.currentPage = 1;
    this.searchTerm = '';
    this.statusFilter = 'All';
    this.loadCandidates();
  }

  loadCandidates(): void {
    if (this.selectedProjectId === null) {
      this.candidates = [];
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.candidateService
      .getCandidatesByProject(this.selectedProjectId)
      .subscribe({
        next: (data) => {
          this.candidates = data;
          this.loading = false;
          this.currentPage = 1;
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.candidates = [];
          this.loading = false;
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to load project candidates.'
          );
          this.cdr.detectChanges();
        }
      });
  }

  get filteredCandidates(): ProjectCandidate[] {
    const term = this.searchTerm.trim().toLowerCase();

    return this.candidates.filter((candidate) => {
      const matchesSearch =
        !term ||
        candidate.employeeName.toLowerCase().includes(term) ||
        candidate.employeeCode.toLowerCase().includes(term) ||
        candidate.projectName.toLowerCase().includes(term);

      const matchesStatus =
        this.statusFilter === 'All' ||
        candidate.status === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
  }

  get paginatedCandidates(): ProjectCandidate[] {
    const start = (this.currentPage - 1) * this.pageSize;

    return this.filteredCandidates.slice(
      start,
      start + this.pageSize
    );
  }

  get totalPages(): number {
    return Math.max(
      1,
      Math.ceil(this.filteredCandidates.length / this.pageSize)
    );
  }

  get totalCandidates(): number {
    return this.candidates.length;
  }

  get selectedCount(): number {
    return this.candidates.filter(
      (candidate) => candidate.status === 'Selected'
    ).length;
  }

  get interviewCount(): number {
    return this.candidates.filter(
      (candidate) => candidate.status === 'InterviewScheduled'
    ).length;
  }

  get rejectedCount(): number {
    return this.candidates.filter(
      (candidate) => candidate.status === 'Rejected'
    ).length;
  }

  onSearchChange(): void {
    this.currentPage = 1;
  }

  onStatusFilterChange(): void {
    this.currentPage = 1;
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  openAddModal(): void {
    if (this.selectedProjectId === null) {
      this.errorMessage = 'Please select a project first.';
      return;
    }

    this.modalType = 'add';
    this.selectedEmployeeCode = '';
    this.addRemarks = '';
    this.availableEmployees = [];

    this.resourcePoolService
      .getAvailableEmployees(this.selectedProjectId)
      .subscribe({
        next: (employees) => {
          const existingCodes = new Set(
            this.candidates.map((candidate) => candidate.employeeCode)
          );

          this.availableEmployees = employees.filter(
            (employee) => !existingCodes.has(employee.employeeCode)
          );

          this.cdr.detectChanges();
        },
        error: (error) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to load available employees.'
          );
          this.closeModal();
          this.cdr.detectChanges();
        }
      });
  }

  addCandidate(): void {
    if (
      this.selectedProjectId === null ||
      !this.selectedEmployeeCode
    ) {
      return;
    }

    const payload: CreateProjectCandidate = {
      employeeCode: this.selectedEmployeeCode,
      projectId: this.selectedProjectId,
      remarks: this.addRemarks.trim()
    };

    this.saving = true;
    this.errorMessage = '';

    this.candidateService.addCandidate(payload).subscribe({
      next: () => {
        this.saving = false;
        this.closeModal();
        this.successMessage = 'Candidate added successfully.';
        this.loadCandidates();
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.saving = false;
        this.errorMessage = this.getErrorMessage(
          error,
          'Unable to add candidate.'
        );
        this.cdr.detectChanges();
      }
    });
  }

  openStatusModal(candidate: ProjectCandidate): void {
    this.selectedCandidate = candidate;
    this.editStatus = this.isValidStatus(candidate.status)
      ? candidate.status
      : 'Selected';

    this.editRole = '';
    this.editInterviewDate = this.toDateTimeLocal(
      candidate.interviewDate
    );
    this.editRemarks = candidate.remarks ?? '';

    this.modalType = 'status';
    this.errorMessage = '';
  }

  updateStatus(): void {
    if (!this.selectedCandidate) {
      return;
    }

    if (
      this.editStatus === 'InterviewScheduled' &&
      (!this.editRole.trim() || !this.editInterviewDate)
    ) {
      this.errorMessage =
        'Role and interview date are required when scheduling an interview.';
      return;
    }

    const payload: UpdateCandidateStatus = {
      status: this.editStatus,
      role: this.editRole.trim(),
      interviewDate:
        this.editStatus === 'InterviewScheduled'
          ? new Date(this.editInterviewDate).toISOString()
          : null,
      remarks: this.editRemarks.trim()
    };

    this.saving = true;
    this.errorMessage = '';

    this.candidateService
      .updateCandidateStatus(
        this.selectedCandidate.candidateId,
        payload
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.closeModal();
          this.successMessage = 'Candidate status updated successfully.';
          this.loadCandidates();
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.saving = false;
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to update candidate status.'
          );
          this.cdr.detectChanges();
        }
      });
  }

  openDetails(candidate: ProjectCandidate): void {
    this.selectedCandidate = candidate;
    this.modalType = 'details';
  }

  closeModal(): void {
    this.modalType = null;
    this.selectedCandidate = null;
    this.errorMessage = '';
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Selected':
        return 'status-selected';
      case 'Rejected':
        return 'status-rejected';
      case 'InterviewScheduled':
        return 'status-interview';
      default:
        return 'status-default';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'InterviewScheduled':
        return 'Interview Scheduled';
      default:
        return status;
    }
  }

  formatDate(date: string | null): string {
    if (!date) {
      return 'Not scheduled';
    }

    return new Date(date).toLocaleString();
  }

  private isValidStatus(status: string): status is CandidateStatus {
    return this.statuses.includes(status as CandidateStatus);
  }

  private toDateTimeLocal(date: string | null): string {
    if (!date) {
      return '';
    }

    const parsed = new Date(date);

    if (Number.isNaN(parsed.getTime())) {
      return '';
    }

    const offset = parsed.getTimezoneOffset() * 60000;

    return new Date(parsed.getTime() - offset)
      .toISOString()
      .slice(0, 16);
  }

  private getErrorMessage(error: any, fallback: string): string {
    return (
      error?.error?.message ||
      error?.error?.title ||
      error?.error ||
      fallback
    );
  }
}
