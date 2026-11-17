
import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardResponse } from '../../core/models/dashboard-response.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {

  private dashboardService = inject(DashboardService);
  private cdr = inject(ChangeDetectorRef);

  dashboardSummary: DashboardResponse | null = null;

  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getDashboardSummary().subscribe({

      next: (response) => {
        this.dashboardSummary = response;
        this.isLoading = false;

        this.cdr.markForCheck();
      },

      error: (error) => {
        console.error('Dashboard API error:', error);

        this.errorMessage = 'Unable to load dashboard data.';
        this.isLoading = false;

        this.cdr.markForCheck();
      }

    });
  }

}
