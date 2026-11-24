
import { Routes } from '@angular/router';

import { MainLayout } from './layout/main-layout/main-layout';
import { Dashboard } from './features/dashboard/dashboard';
import { Employees } from './features/employees/employees';
import { Skills } from './features/skills/skills';
import { Projects } from './features/projects/projects';
import { ProjectSkills } from './features/project-skills/project-skills';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'dashboard',
        component: Dashboard
      },
      {
        path: 'employees',
        component: Employees
      },
      {
        path: 'skills',
        component: Skills
      },
      {
        path: 'projects',
        component: Projects
      },
      {
        path: 'project-skills',
        component: ProjectSkills
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
