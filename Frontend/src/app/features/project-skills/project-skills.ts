
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { ProjectService } from '../../core/services/project.service';
import { SkillService } from '../../core/services/skill.service';
import { ProjectSkillService } from '../../core/services/project-skill.service';

import { ProjectResponse } from '../../core/models/project.model';
import { SkillResponse } from '../../core/models/skill.model';
import { ProjectSkillResponse } from '../../core/models/project-skill.model';

@Component({
  selector: 'app-project-skills',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './project-skills.html',
  styleUrl: './project-skills.scss'
})
export class ProjectSkills implements OnInit {

  projects: ProjectResponse[] = [];
  allSkills: SkillResponse[] = [];
  assignedSkills: ProjectSkillResponse[] = [];

  selectedProjectId: number | null = null;
  selectedSkillIds: number[] = [];

  loadingProjects = false;
  loadingSkills = false;
  loadingAssignments = false;
  saving = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private projectService: ProjectService,
    private skillService: SkillService,
    private projectSkillService: ProjectSkillService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProjects();
    this.loadAllSkills();
  }

  // LOAD ALL PROJECTS
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

        this.errorMessage = 'Unable to load projects.';
        this.loadingProjects = false;

        this.cdr.detectChanges();
      }
    });
  }

  // LOAD ALL AVAILABLE SKILLS
  loadAllSkills(): void {
    this.loadingSkills = true;

    this.skillService.getSkills().subscribe({
      next: (skills) => {
        this.allSkills = skills;
        this.loadingSkills = false;

        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Unable to load skills:', error);

        this.errorMessage = 'Unable to load available skills.';
        this.loadingSkills = false;

        this.cdr.detectChanges();
      }
    });
  }

  // PROJECT SELECTION
  onProjectChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    this.selectedProjectId = value ? Number(value) : null;

    this.assignedSkills = [];
    this.selectedSkillIds = [];

    this.errorMessage = '';
    this.successMessage = '';

    if (this.selectedProjectId !== null) {
      this.loadAssignedSkills(this.selectedProjectId);
    }
  }

  // LOAD SKILLS ASSIGNED TO SELECTED PROJECT
  loadAssignedSkills(projectId: number): void {
    this.loadingAssignments = true;

    this.projectSkillService.getProjectSkills(projectId).subscribe({
      next: (skills) => {
        this.assignedSkills = skills;
        this.loadingAssignments = false;

        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Unable to load project skills:', error);

        this.errorMessage = 'Unable to load project skills.';
        this.loadingAssignments = false;

        this.cdr.detectChanges();
      }
    });
  }

  // GET SKILLS NOT YET ASSIGNED TO PROJECT
  get availableSkills(): SkillResponse[] {
    const assignedIds = new Set(
      this.assignedSkills.map(skill => skill.skillId)
    );

    return this.allSkills.filter(
      skill => !assignedIds.has(skill.skillId)
    );
  }

  // SELECT OR DESELECT A SKILL
  toggleSkill(skillId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;

    if (checked) {
      if (!this.selectedSkillIds.includes(skillId)) {
        this.selectedSkillIds = [
          ...this.selectedSkillIds,
          skillId
        ];
      }
    } else {
      this.selectedSkillIds = this.selectedSkillIds.filter(
        id => id !== skillId
      );
    }

    this.successMessage = '';
    this.errorMessage = '';
  }

  // CHECK WHETHER A SKILL IS SELECTED
  isSkillSelected(skillId: number): boolean {
    return this.selectedSkillIds.includes(skillId);
  }

  // SAVE SELECTED PROJECT SKILLS
  saveProjectSkills(): void {
    if (
      this.selectedProjectId === null ||
      this.selectedSkillIds.length === 0 ||
      this.saving
    ) {
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.projectSkillService
      .assignProjectSkills(
        this.selectedProjectId,
        this.selectedSkillIds
      )
      .subscribe({
        next: (skills) => {
          this.assignedSkills = skills;
          this.selectedSkillIds = [];

          this.successMessage =
            'Project skills assigned successfully.';

          this.saving = false;

          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Unable to assign project skills:', error);

          this.errorMessage =
            error?.error?.message ||
            'Unable to assign project skills.';

          this.saving = false;

          this.cdr.detectChanges();
        }
      });
  }

  // CLEAR PENDING SKILL SELECTION
  clearSelection(): void {
    this.selectedSkillIds = [];
    this.successMessage = '';
    this.errorMessage = '';
  }

  // GET SELECTED PROJECT DETAILS
  get selectedProject(): ProjectResponse | undefined {
    return this.projects.find(
      project => project.projectId === this.selectedProjectId
    );
  }
}
