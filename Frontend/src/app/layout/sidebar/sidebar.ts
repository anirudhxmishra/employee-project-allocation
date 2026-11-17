
import { Component } from '@angular/core';
import {
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import {
  LucideHouse,
  LucideUsers,
  LucideGraduationCap,
  LucideBriefcaseBusiness,
  LucideUserRoundSearch,
  LucideClipboardList,
  LucideCalendarCheck
} from '@lucide/angular';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    LucideHouse,
    LucideUsers,
    LucideGraduationCap,
    LucideBriefcaseBusiness,
    LucideUserRoundSearch,
    LucideClipboardList,
    LucideCalendarCheck
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar {

  navigationItems = [
    { label: 'Dashboard', route: '/dashboard', icon: 'dashboard' },
    { label: 'Employees', route: '/employees', icon: 'employees' },
    { label: 'Skills', route: '/skills', icon: 'skills' },
    { label: 'Projects', route: '/projects', icon: 'projects' },
    { label: 'Resource Pool', route: '/resource-pool', icon: 'resource-pool' },
    { label: 'Project Candidates', route: '/project-candidates', icon: 'candidates' },
    { label: 'Allocations', route: '/allocations', icon: 'allocations' }
  ];

}
