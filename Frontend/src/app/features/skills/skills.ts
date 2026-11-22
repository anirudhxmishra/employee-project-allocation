
import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { SkillService } from '../../core/services/skill.service';
import { EmployeeService } from '../../core/services/employee.service';

import {
  SkillResponse,
  CreateSkill
} from '../../core/models/skill.model';

import { EmployeeResponse } from '../../core/models/employee-response.model';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './skills.html',
  styleUrl: './skills.scss'
})
export class Skills implements OnInit {

  private skillService = inject(SkillService);
  private employeeService = inject(EmployeeService);
  private cdr = inject(ChangeDetectorRef);

  skills: SkillResponse[] = [];
  employees: EmployeeResponse[] = [];
  employeeSkills: SkillResponse[] = [];

  // Skills selected but not yet saved
  pendingSkills: SkillResponse[] = [];

  draggedSkillId: number | null = null;

  searchTerm = '';
  skillName = '';
  selectedEmployeeCode = '';

  isLoading = false;
  isSubmitting = false;
  isLoadingEmployees = false;
  isLoadingEmployeeSkills = false;
  isSavingAssignments = false;

  showAddForm = false;

  successMessage = '';
  errorMessage = '';
  employeeSkillsError = '';
  assignmentError = '';

  ngOnInit(): void {
    this.loadSkills();
    this.loadEmployees();
  }

  // Get all skills
  loadSkills(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.skillService.getSkills().subscribe({
      next: (response) => {
        this.skills = response;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        this.errorMessage = this.getErrorMessage(
          error,
          'Unable to load skills. Please try again.'
        );
        this.cdr.markForCheck();
      }
    });
  }

  // Get all employees
  loadEmployees(): void {
    this.isLoadingEmployees = true;

    this.employeeService.getEmployees().subscribe({
      next: (response) => {
        this.employees = response;
        this.isLoadingEmployees = false;
        this.cdr.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.isLoadingEmployees = false;
        this.employeeSkillsError = this.getErrorMessage(
          error,
          'Unable to load employees.'
        );
        this.cdr.markForCheck();
      }
    });
  }

  // Load skills when an employee is selected
  onEmployeeChange(): void {
    this.employeeSkills = [];
    this.pendingSkills = [];
    this.assignmentError = '';
    this.employeeSkillsError = '';

    if (!this.selectedEmployeeCode) {
      this.cdr.markForCheck();
      return;
    }

    this.isLoadingEmployeeSkills = true;

    this.skillService
      .getEmployeeSkills(this.selectedEmployeeCode)
      .subscribe({
        next: (response) => {
          this.employeeSkills = response;
          this.isLoadingEmployeeSkills = false;
          this.cdr.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.isLoadingEmployeeSkills = false;
          this.employeeSkillsError = this.getErrorMessage(
            error,
            'Unable to load employee skills.'
          );
          this.cdr.markForCheck();
        }
      });
  }

  // Skills available to assign
  get availableSkills(): SkillResponse[] {
    const assignedIds = [
      ...this.employeeSkills,
      ...this.pendingSkills
    ].map(skill => skill.skillId);

    return this.skills.filter(
      skill => !assignedIds.includes(skill.skillId)
    );
  }

  // Existing and newly selected skills
  get displayedAssignedSkills(): SkillResponse[] {
    return [
      ...this.employeeSkills,
      ...this.pendingSkills
    ];
  }

  // Start dragging a skill
  onDragStart(skillId: number): void {
    this.draggedSkillId = skillId;
  }

  // Allow dropping
  onDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  // Drop a skill into Assigned Skills
  dropOnAssigned(event: DragEvent): void {
    event.preventDefault();

    if (this.draggedSkillId === null) return;

    const skill = this.availableSkills.find(
      item => item.skillId === this.draggedSkillId
    );

    if (
      skill &&
      !this.displayedAssignedSkills.some(
        item => item.skillId === skill.skillId
      )
    ) {
      this.pendingSkills = [...this.pendingSkills, skill];
    }

    this.draggedSkillId = null;
    this.cdr.markForCheck();
  }

  // Drop a newly selected skill back into Available Skills
  dropOnAvailable(event: DragEvent): void {
    event.preventDefault();

    if (this.draggedSkillId === null) return;

    this.pendingSkills = this.pendingSkills.filter(
      skill => skill.skillId !== this.draggedSkillId
    );

    this.draggedSkillId = null;
    this.cdr.markForCheck();
  }

  // Cancel pending changes
  cancelAssignmentChanges(): void {
    this.pendingSkills = [];
    this.assignmentError = '';
    this.draggedSkillId = null;
    this.cdr.markForCheck();
  }

  // Save new skill assignments
  saveAssignments(): void {
    if (
      !this.selectedEmployeeCode ||
      this.pendingSkills.length === 0 ||
      this.isSavingAssignments
    ) {
      return;
    }

    const skillIds = this.pendingSkills.map(
      skill => skill.skillId
    );

    this.isSavingAssignments = true;
    this.assignmentError = '';
    this.successMessage = '';

    this.skillService
      .assignSkills(this.selectedEmployeeCode, skillIds)
      .subscribe({
        next: () => {
          this.isSavingAssignments = false;
          this.pendingSkills = [];
          this.successMessage = 'Skills assigned successfully.';

          this.cdr.markForCheck();

          // Refresh the employee's assigned skills
          this.onEmployeeChange();
        },
        error: (error: HttpErrorResponse) => {
          this.isSavingAssignments = false;
          this.assignmentError = this.getErrorMessage(
            error,
            'Unable to assign skills. Please try again.'
          );
          this.cdr.markForCheck();
        }
      });
  }

  // Filter skills
  get filteredSkills(): SkillResponse[] {
    const search = this.searchTerm.trim().toLowerCase();

    if (!search) {
      return this.skills;
    }

    return this.skills.filter(skill =>
      skill.skillName.toLowerCase().includes(search)
    );
  }

  // Open add skill form
  openAddForm(): void {
    this.skillName = '';
    this.successMessage = '';
    this.errorMessage = '';
    this.showAddForm = true;
    this.cdr.markForCheck();
  }

  // Close add skill form
  closeAddForm(): void {
    if (this.isSubmitting) return;

    this.showAddForm = false;
    this.skillName = '';
    this.cdr.markForCheck();
  }

  // Create skill
  createSkill(): void {
    const name = this.skillName.trim();

    if (name.length < 2) {
      this.errorMessage =
        'Skill name must contain at least 2 characters.';
      this.cdr.markForCheck();
      return;
    }

    if (name.length > 100) {
      this.errorMessage =
        'Skill name cannot exceed 100 characters.';
      this.cdr.markForCheck();
      return;
    }

    const skill: CreateSkill = {
      skillName: name
    };

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.markForCheck();

    this.skillService.createSkill(skill).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showAddForm = false;
        this.skillName = '';
        this.successMessage = 'Skill added successfully.';

        this.cdr.markForCheck();
        this.loadSkills();
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting = false;
        this.errorMessage = this.getErrorMessage(
          error,
          'Unable to add skill. Please try again.'
        );
        this.cdr.markForCheck();
      }
    });
  }

  // Extract backend error message
  private getErrorMessage(
    error: HttpErrorResponse,
    fallback: string
  ): string {
    const message =
      error.error?.message ||
      error.error?.title ||
      error.error?.detail;

    return typeof message === 'string' && message.trim()
      ? message
      : fallback;
  }

  // Clear messages
  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.markForCheck();
  }
}
