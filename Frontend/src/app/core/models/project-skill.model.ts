
export interface ProjectSkillResponse {
  projectId: number;
  skillId: number;
  skillName: string;
}

export interface AssignProjectSkills {
  skillIds: number[];
}
