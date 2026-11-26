import { CommonModule } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  HostListener
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AllocationService } from '../../core/services/allocation.service';
import { Allocation, CreateAllocation } from '../../core/models/allocation.model';

interface ProjectOption {
  projectId: number;
  projectCode: string;
  projectName: string;
  status: string;
}

interface EmployeeOption {
  employeeCode: string;
  fullName: string;
  location: string | null;
  trainingName: string | null;
  skills: string[];
}

@Component({
  selector: 'app-allocations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './allocations.html',
  styleUrl: './allocations.scss'
})
export class Allocations implements OnInit {

  allocations: Allocation[] = [];
  projects: ProjectOption[] = [];
  availableEmployees: EmployeeOption[] = [];
  employeeHistory: Allocation[] = [];

  loading = false;
  loadingEmployees = false;
  loadingHistory = false;
  saving = false;
  releasingId: number | null = null;

  errorMessage = '';
  successMessage = '';

  searchTerm = '';
  statusFilter = 'All';
  projectFilter = 'All';

  currentPage = 1;
  pageSize = 8;

  showAddModal = false;
  showDetailsModal = false;
  showHistoryModal = false;
  showReleaseModal = false;
  selectedAllocation: Allocation | null = null;
  allocationToRelease: Allocation | null = null;

  newAllocation: CreateAllocation = this.getEmptyForm();

  constructor(
    private allocationService: AllocationService,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadAllocations();
    this.loadProjects();
  }

  // CLOSE MODALS WITH ESCAPE KEY
  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.showReleaseModal) {
      this.cancelRelease();
      return;
    }

    if (this.showAddModal || this.showDetailsModal || this.showHistoryModal) {
      this.closeModals();
    }
  }

  private getEmptyForm(): CreateAllocation {
    return {
      employeeCode: '',
      projectId: 0,
      role: '',
      allocatedBy: 'HR Admin'
    };
  }

  // ---------- DATA LOADING ----------

  loadAllocations(): void {
    this.loading = true;
    this.errorMessage = '';

    this.allocationService.getAllAllocations().subscribe({
      next: (data) => {
        this.allocations = data ?? [];
        this.loading = false;

        // keep current page valid if the list shrank
        if (this.currentPage > this.totalPages) {
          this.currentPage = this.totalPages;
        }

        this.cdr.detectChanges();
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage =
          error.error?.message || 'Unable to load allocations.';
        this.cdr.detectChanges();
      }
    });
  }

  loadProjects(): void {
    this.http.get<ProjectOption[]>('/api/Projects').subscribe({
      next: (data) => {
        this.projects = data ?? [];
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Unable to load projects.';
        this.cdr.detectChanges();
      }
    });
  }

  // ---------- FILTERING & PAGINATION ----------

  get filteredAllocations(): Allocation[] {
    const search = this.searchTerm.trim().toLowerCase();

    return this.allocations.filter((allocation) => {
      const matchesSearch =
        !search ||
        allocation.employeeName.toLowerCase().includes(search) ||
        allocation.employeeCode.toLowerCase().includes(search) ||
        allocation.projectName.toLowerCase().includes(search) ||
        allocation.role.toLowerCase().includes(search);

      const matchesStatus =
        this.statusFilter === 'All' ||
        (this.statusFilter === 'Active' && allocation.isActive) ||
        (this.statusFilter === 'Released' && !allocation.isActive);

      const matchesProject =
        this.projectFilter === 'All' ||
        allocation.projectId.toString() === this.projectFilter;

      return matchesSearch && matchesStatus && matchesProject;
    });
  }

  get paginatedAllocations(): Allocation[] {
    const start = (this.currentPage - 1) * this.pageSize;

    return this.filteredAllocations.slice(
      start,
      start + this.pageSize
    );
  }

  get totalPages(): number {
    return Math.max(
      1,
      Math.ceil(this.filteredAllocations.length / this.pageSize)
    );
  }

  get pageStart(): number {
    return this.filteredAllocations.length
      ? (this.currentPage - 1) * this.pageSize + 1
      : 0;
  }

  get pageEnd(): number {
    return Math.min(
      this.currentPage * this.pageSize,
      this.filteredAllocations.length
    );
  }

  get hasActiveFilters(): boolean {
    return !!(
      this.searchTerm ||
      this.statusFilter !== 'All' ||
      this.projectFilter !== 'All'
    );
  }

  get activeCount(): number {
    return this.allocations.filter(a => a.isActive).length;
  }

  get releasedCount(): number {
    return this.allocations.filter(a => !a.isActive).length;
  }

  get totalEmployeesAllocated(): number {
    return new Set(
      this.allocations
        .filter(a => a.isActive)
        .map(a => a.employeeCode)
    ).size;
  }

  onFilterChange(): void {
    this.currentPage = 1;
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = 'All';
    this.projectFilter = 'All';
    this.currentPage = 1;
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  // ---------- CREATE ----------

  openAddModal(): void {
    this.newAllocation = this.getEmptyForm();
    this.availableEmployees = [];
    this.errorMessage = '';
    this.successMessage = '';
    this.showAddModal = true;
  }

  onProjectSelected(): void {
    this.newAllocation.employeeCode = '';
    this.availableEmployees = [];

    if (!this.newAllocation.projectId) {
      return;
    }

    this.loadingEmployees = true;

    const params = new HttpParams().set(
      'projectId',
      this.newAllocation.projectId
    );

    this.http.get<EmployeeOption[]>('/api/ResourcePool', { params })
      .subscribe({
        next: (employees) => {
          const activeEmployeeCodes = new Set(
            this.allocations
              .filter(a => a.isActive)
              .map(a => a.employeeCode)
          );

          this.availableEmployees = (employees ?? []).filter(
            employee => !activeEmployeeCodes.has(employee.employeeCode)
          );

          this.loadingEmployees = false;
          this.cdr.detectChanges();
        },
        error: () => {
          this.loadingEmployees = false;
          this.errorMessage = 'Unable to load available employees.';
          this.cdr.detectChanges();
        }
      });
  }

  createAllocation(): void {
    if (this.saving) {
      return;
    }

    if (
      !this.newAllocation.employeeCode ||
      !this.newAllocation.projectId ||
      !this.newAllocation.role.trim()
    ) {
      this.errorMessage = 'Please complete all required fields.';
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    this.allocationService.createAllocation(this.newAllocation).subscribe({
      next: () => {
        this.saving = false;
        this.showAddModal = false;
        this.successMessage = 'Employee allocated successfully.';
        this.loadAllocations();
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.saving = false;
        this.errorMessage =
          error.error?.message || 'Unable to create allocation.';
        this.cdr.detectChanges();
      }
    });
  }

  // ---------- RELEASE ----------

  // Opens the confirmation popup
  releaseAllocation(allocation: Allocation): void {
    if (!allocation.isActive || this.releasingId !== null) {
      return;
    }

    this.allocationToRelease = allocation;
    this.showReleaseModal = true;
  }

  // Called when the user confirms in the popup
  confirmRelease(): void {
    const allocation = this.allocationToRelease;

    if (!allocation || this.releasingId !== null) {
      return;
    }

    this.releasingId = allocation.allocationId;
    this.clearMessages();

    const releaseDate = new Date().toISOString();

    this.allocationService.releaseAllocation(
      allocation.allocationId,
      { releaseDate }
    ).subscribe({
      next: () => {
        this.releasingId = null;
        this.showReleaseModal = false;
        this.allocationToRelease = null;
        this.successMessage =
          `${allocation.employeeName} has been released successfully.`;
        this.loadAllocations();
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.releasingId = null;
        this.showReleaseModal = false;
        this.allocationToRelease = null;
        this.errorMessage =
          error.error?.message || 'Unable to release allocation.';
        this.cdr.detectChanges();
      }
    });
  }

  cancelRelease(): void {
    if (this.releasingId !== null) {
      return;
    }

    this.showReleaseModal = false;
    this.allocationToRelease = null;
  }

  // ---------- DETAILS & HISTORY ----------

  openDetails(allocation: Allocation): void {
    this.selectedAllocation = allocation;
    this.showDetailsModal = true;
  }

  openEmployeeHistory(allocation: Allocation): void {
    this.selectedAllocation = allocation;
    this.employeeHistory = [];
    this.loadingHistory = true;
    this.showHistoryModal = true;

    this.allocationService
      .getEmployeeAllocations(allocation.employeeCode)
      .subscribe({
        next: (data) => {
          this.employeeHistory = data ?? [];
          this.loadingHistory = false;
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.loadingHistory = false;
          this.errorMessage =
            error.error?.message || 'Unable to load employee history.';
          this.cdr.detectChanges();
        }
      });
  }

  closeModals(): void {
    this.showAddModal = false;
    this.showDetailsModal = false;
    this.showHistoryModal = false;
    this.selectedAllocation = null;
    this.errorMessage = '';
  }

  // ---------- HELPERS ----------

  getInitial(name: string): string {
    return (name || '?').charAt(0).toUpperCase();
  }

  /** Date with time, used in the details modal */
  formatDate(date: string | null): string {
    if (!date) {
      return '—';
    }

    return new Date(date).toLocaleString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  /** Date only, used in the table and history list */
  formatShortDate(date: string | null): string {
    if (!date) {
      return '—';
    }

    return new Date(date).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  trackByAllocation(_: number, allocation: Allocation): number {
    return allocation.allocationId;
  }

  trackByProject(_: number, project: ProjectOption): number {
    return project.projectId;
  }
}
