import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';

import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { SkillService } from '../../core/services/skill.service';
import { EmployeeService } from '../../core/services/employee.service';

import {
  SkillResponse,
  CreateSkill
} from '../../core/models/skill.model';

import { EmployeeResponse } from '../../core/models/employee-response.model';

type DropTarget = 'available' | 'assigned';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './skills.html',
  styleUrl: './skills.scss'
})
export class Skills implements OnInit, OnDestroy {

  private skillService = inject(SkillService);
  private employeeService = inject(EmployeeService);
  private cdr = inject(ChangeDetectorRef);

  // Focus the skill name field when the Add Skill form opens
  @ViewChild('skillInput') private skillInput?: ElementRef<HTMLInputElement>;

  skills: SkillResponse[] = [];
  employees: EmployeeResponse[] = [];
  employeeSkills: SkillResponse[] = [];

  // Skills selected but not yet saved
  pendingSkills: SkillResponse[] = [];

  // Drag and drop state
  draggedSkillId: number | null = null;
  dropTarget: DropTarget | null = null;

  searchTerm = '';
  availableSearch = '';
  skillName = '';
  selectedEmployeeCode = '';

  isLoading = true;
  isSubmitting = false;
  isLoadingEmployees = false;
  isLoadingEmployeeSkills = false;
  isSavingAssignments = false;

  showAddForm = false;

  successMessage = '';
  errorMessage = '';
  formError = '';
  employeeSkillsError = '';
  assignmentError = '';

  private successTimer: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.loadSkills();
    this.loadEmployees();
  }

  ngOnDestroy(): void {
    this.clearSuccessTimer();
  }

  // Close the Add Skill form with the Escape key
  @HostListener('document:keydown.escape')
  onEscapePressed(): void {
    if (this.showAddForm) {
      this.closeAddForm();
    }
  }

  // Get all skills
  loadSkills(silent = false): void {
    if (!silent) {
      this.isLoading = true;
    }

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

  // First letter or digit of a skill name, used for the avatar (".NET" -> "N")
  getInitial(skillName: string): string {
    const match = skillName.match(/[A-Za-z0-9]/);

    return (match ? match[0] : skillName.charAt(0)).toUpperCase();
  }

  // Employee currently selected in the assignment card
  get selectedEmployee(): EmployeeResponse | null {
    return (
      this.employees.find(
        employee => employee.employeeCode === this.selectedEmployeeCode
      ) ?? null
    );
  }

  // Load skills when an employee is selected
  onEmployeeSelect(employeeCode: string): void {
    this.selectedEmployeeCode = employeeCode ?? '';

    this.employeeSkills = [];
    this.pendingSkills = [];
    this.availableSearch = '';
    this.assignmentError = '';
    this.employeeSkillsError = '';
    this.isLoadingEmployeeSkills = false;
    this.draggedSkillId = null;
    this.dropTarget = null;

    if (!this.selectedEmployeeCode) {
      this.cdr.markForCheck();
      return;
    }

    this.fetchEmployeeSkills(this.selectedEmployeeCode);
  }

  // Fetch the skills already assigned to an employee
  private fetchEmployeeSkills(employeeCode: string, silent = false): void {
    if (!silent) {
      this.isLoadingEmployeeSkills = true;
    }

    this.skillService.getEmployeeSkills(employeeCode).subscribe({
      next: (response) => {
        // Ignore the response if another employee was selected meanwhile
        if (this.selectedEmployeeCode !== employeeCode) return;

        this.employeeSkills = response;
        this.isLoadingEmployeeSkills = false;
        this.cdr.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        if (this.selectedEmployeeCode !== employeeCode) return;

        this.isLoadingEmployeeSkills = false;

        // A silent refresh keeps the data already on screen
        if (!silent) {
          this.employeeSkillsError = this.getErrorMessage(
            error,
            'Unable to load employee skills.'
          );
        }

        this.cdr.markForCheck();
      }
    });
  }

  // Skills available to assign
  get availableSkills(): SkillResponse[] {
    const assignedIds = new Set(
      [...this.employeeSkills, ...this.pendingSkills].map(
        skill => skill.skillId
      )
    );

    return this.skills.filter(skill => !assignedIds.has(skill.skillId));
  }

  // Available skills after applying the panel filter
  get filteredAvailableSkills(): SkillResponse[] {
    const search = this.availableSearch.trim().toLowerCase();

    if (!search) {
      return this.availableSkills;
    }

    return this.availableSkills.filter(skill =>
      skill.skillName.toLowerCase().includes(search)
    );
  }

  // Existing and newly selected skills
  get displayedAssignedSkills(): SkillResponse[] {
    return [
      ...this.employeeSkills,
      ...this.pendingSkills
    ];
  }

  // Add a skill to the pending list (drop or + button)
  addPendingSkill(skill: SkillResponse): void {
    const alreadyAssigned = this.displayedAssignedSkills.some(
      item => item.skillId === skill.skillId
    );

    if (alreadyAssigned) return;

    this.pendingSkills = [...this.pendingSkills, skill];
    this.assignmentError = '';
    this.cdr.markForCheck();
  }

  // Remove a skill from the pending list (drop back or x button)
  removePendingSkill(skillId: number): void {
    this.pendingSkills = this.pendingSkills.filter(
      skill => skill.skillId !== skillId
    );

    this.cdr.markForCheck();
  }

  // Start dragging a skill
  onDragStart(event: DragEvent, skillId: number): void {
    this.draggedSkillId = skillId;

    // Required by Firefox for the drag to start
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
      event.dataTransfer.setData('text/plain', String(skillId));
    }
  }

  // Finish dragging (drop anywhere or cancelled)
  onDragEnd(): void {
    this.resetDragState();
  }

  // Allow dropping only where the dragged skill can actually go
  onDragOver(event: DragEvent, target: DropTarget): void {
    if (!this.canDropOn(target)) return;

    event.preventDefault();

    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }

    if (this.dropTarget !== target) {
      this.dropTarget = target;
      this.cdr.markForCheck();
    }
  }

  // Remove the highlight when the pointer leaves a panel
  onDragLeave(event: DragEvent, target: DropTarget): void {
    const panel = event.currentTarget as HTMLElement;
    const next = event.relatedTarget as Node | null;

    // Ignore leave events fired when moving over child elements
    if (next && panel.contains(next)) return;

    if (this.dropTarget === target) {
      this.dropTarget = null;
      this.cdr.markForCheck();
    }
  }

  // Drop a skill into Assigned Skills
  dropOnAssigned(event: DragEvent): void {
    event.preventDefault();

    const skillId = this.draggedSkillId;
    this.resetDragState();

    if (skillId === null) return;

    const skill = this.availableSkills.find(
      item => item.skillId === skillId
    );

    if (skill) {
      this.addPendingSkill(skill);
    }
  }

  // Drop a newly selected skill back into Available Skills
  dropOnAvailable(event: DragEvent): void {
    event.preventDefault();

    const skillId = this.draggedSkillId;
    this.resetDragState();

    if (skillId === null) return;

    this.removePendingSkill(skillId);
  }

  // Available skills can only go to Assigned, pending skills only back
  private canDropOn(target: DropTarget): boolean {
    if (this.draggedSkillId === null) return false;

    const isPending = this.pendingSkills.some(
      skill => skill.skillId === this.draggedSkillId
    );

    return target === 'assigned' ? !isPending : isPending;
  }

  private resetDragState(): void {
    this.draggedSkillId = null;
    this.dropTarget = null;
    this.cdr.markForCheck();
  }

  // Cancel pending changes
  cancelAssignmentChanges(): void {
    this.pendingSkills = [];
    this.assignmentError = '';
    this.resetDragState();
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

    const employeeCode = this.selectedEmployeeCode;
    const assignedSkills = this.pendingSkills;
    const skillIds = assignedSkills.map(skill => skill.skillId);

    this.isSavingAssignments = true;
    this.assignmentError = '';
    this.clearMessages();

    this.skillService
      .assignSkills(employeeCode, skillIds)
      .subscribe({
        next: () => {
          this.isSavingAssignments = false;

          // Show the saved skills straight away, then sync with the server
          this.employeeSkills = [...this.employeeSkills, ...assignedSkills];
          this.pendingSkills = [];

          this.showSuccess('Skills assigned successfully.');
          this.cdr.markForCheck();

          this.fetchEmployeeSkills(employeeCode, true);
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
    this.formError = '';
    this.successMessage = '';
    this.clearSuccessTimer();
    this.showAddForm = true;
    this.cdr.markForCheck();

    setTimeout(() => this.skillInput?.nativeElement.focus());
  }

  // Close add skill form
  closeAddForm(): void {
    if (this.isSubmitting) return;

    this.showAddForm = false;
    this.skillName = '';
    this.formError = '';
    this.cdr.markForCheck();
  }

  // Create skill
  createSkill(): void {
    if (this.isSubmitting) return;

    const name = this.skillName.trim();

    if (name.length < 2) {
      this.formError =
        'Skill name must contain at least 2 characters.';
      this.cdr.markForCheck();
      return;
    }

    if (name.length > 100) {
      this.formError =
        'Skill name cannot exceed 100 characters.';
      this.cdr.markForCheck();
      return;
    }

    const skill: CreateSkill = {
      skillName: name
    };

    this.isSubmitting = true;
    this.formError = '';
    this.cdr.markForCheck();

    this.skillService.createSkill(skill).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showAddForm = false;
        this.skillName = '';

        this.showSuccess('Skill added successfully.');
        this.cdr.markForCheck();

        // Refresh the list without showing the loading spinner
        this.loadSkills(true);
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting = false;
        this.formError = this.getErrorMessage(
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

  // Show a success message that dismisses itself
  private showSuccess(message: string): void {
    this.clearSuccessTimer();
    this.successMessage = message;

    this.successTimer = setTimeout(() => {
      this.successMessage = '';
      this.successTimer = null;
      this.cdr.markForCheck();
    }, 4000);
  }

  private clearSuccessTimer(): void {
    if (this.successTimer) {
      clearTimeout(this.successTimer);
      this.successTimer = null;
    }
  }

  // Clear messages
  clearMessages(): void {
    this.clearSuccessTimer();
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.markForCheck();
  }
}
