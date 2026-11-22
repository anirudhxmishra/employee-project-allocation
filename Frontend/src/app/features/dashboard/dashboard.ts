import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardResponse } from '../../core/models/dashboard-response.model';

// Single slice of a donut chart
interface ChartSlice {
  label: string;
  value: number;
  percent: number;
  color: string;
  dashArray: string;
  dashOffset: number;
}

// Donut chart card
interface DonutChart {
  key: string;
  title: string;
  subtitle: string;
  total: number;
  totalLabel: string;
  slices: ChartSlice[];
}

// Horizontal bar in the overview chart
interface BarItem {
  label: string;
  value: number;
  widthPercent: number;
  color: string;
}

const DONUT_RADIUS = 70;
const DONUT_CIRCUMFERENCE = 2 * Math.PI * DONUT_RADIUS;

// Shades of blue used across the charts
const BLUE_900 = '#1e3a8a';
const BLUE_700 = '#1d4ed8';
const BLUE_600 = '#2563eb';
const BLUE_500 = '#3b82f6';
const BLUE_400 = '#60a5fa';
const BLUE_300 = '#93c5fd';

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

  // Chart data
  readonly donutRadius = DONUT_RADIUS;
  charts: DonutChart[] = [];
  barItems: BarItem[] = [];

  // Hover / focus state
  activeChartKey: string | null = null;
  activeSliceIndex: number | null = null;
  hoveredBarLabel: string | null = null;

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getDashboardSummary().subscribe({

      next: (response) => {
        this.dashboardSummary = response;
        this.buildCharts(response);
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

  // Donut hover / focus handling
  setActiveSlice(chartKey: string, index: number): void {
    this.activeChartKey = chartKey;
    this.activeSliceIndex = index;
  }

  clearActiveSlice(): void {
    this.activeChartKey = null;
    this.activeSliceIndex = null;
  }

  isSliceActive(chartKey: string, index: number): boolean {
    return this.activeChartKey === chartKey &&
      this.activeSliceIndex === index;
  }

  isSliceDimmed(chartKey: string, index: number): boolean {
    return this.activeChartKey === chartKey &&
      this.activeSliceIndex !== null &&
      this.activeSliceIndex !== index;
  }

  getActiveSlice(chart: DonutChart): ChartSlice | null {
    if (
      this.activeChartKey !== chart.key ||
      this.activeSliceIndex === null
    ) {
      return null;
    }

    return chart.slices[this.activeSliceIndex] ?? null;
  }

  // Bar hover handling
  setHoveredBar(label: string): void {
    this.hoveredBarLabel = label;
  }

  clearHoveredBar(): void {
    this.hoveredBarLabel = null;
  }

  // Build all chart data from the dashboard summary
  private buildCharts(summary: DashboardResponse): void {
    const totalEmployees = summary.totalEmployees ?? 0;
    const allocatedEmployees = summary.allocatedEmployees ?? 0;
    const availableEmployees = summary.availableEmployees ?? 0;

    const totalProjects = summary.totalProjects ?? 0;
    const activeProjects = summary.activeProjects ?? 0;

    const totalAllocations = summary.totalAllocations ?? 0;
    const activeAllocations = summary.activeAllocations ?? 0;

    this.charts = [
      this.buildDonut(
        'employees',
        'Employee Allocation',
        'Allocated vs available employees',
        totalEmployees,
        'Employees',
        [
          { label: 'Allocated', value: allocatedEmployees, color: BLUE_900 },
          { label: 'Available', value: availableEmployees, color: BLUE_300 }
        ]
      ),
      this.buildDonut(
        'projects',
        'Project Status',
        'Active vs inactive projects',
        totalProjects,
        'Projects',
        [
          { label: 'Active', value: activeProjects, color: BLUE_600 },
          {
            label: 'Inactive',
            value: Math.max(totalProjects - activeProjects, 0),
            color: BLUE_300
          }
        ]
      ),
      this.buildDonut(
        'allocations',
        'Allocation Status',
        'Active vs inactive allocations',
        totalAllocations,
        'Allocations',
        [
          { label: 'Active', value: activeAllocations, color: BLUE_500 },
          {
            label: 'Inactive',
            value: Math.max(totalAllocations - activeAllocations, 0),
            color: BLUE_300
          }
        ]
      )
    ];

    const bars = [
      { label: 'Total Employees', value: totalEmployees, color: BLUE_700 },
      { label: 'Allocated Employees', value: allocatedEmployees, color: BLUE_700 },
      { label: 'Available Employees', value: availableEmployees, color: BLUE_700 },
      { label: 'Total Projects', value: totalProjects, color: BLUE_500 },
      { label: 'Active Projects', value: activeProjects, color: BLUE_500 },
      { label: 'Total Allocations', value: totalAllocations, color: BLUE_400 },
      { label: 'Active Allocations', value: activeAllocations, color: BLUE_400 }
    ];

    const maxValue = Math.max(...bars.map(bar => bar.value), 0);

    this.barItems = bars.map(bar => ({
      ...bar,
      widthPercent: maxValue > 0 ? (bar.value / maxValue) * 100 : 0
    }));
  }

  // Create donut slices using SVG stroke-dasharray
  private buildDonut(
    key: string,
    title: string,
    subtitle: string,
    total: number,
    totalLabel: string,
    items: { label: string; value: number; color: string }[]
  ): DonutChart {

    const sum = items.reduce((acc, item) => acc + item.value, 0);
    let offset = 0;

    const slices: ChartSlice[] = items.map(item => {
      const fraction = sum > 0 ? item.value / sum : 0;
      const length = fraction * DONUT_CIRCUMFERENCE;

      const slice: ChartSlice = {
        label: item.label,
        value: item.value,
        color: item.color,
        percent: Math.round(fraction * 1000) / 10,
        dashArray: `${length} ${DONUT_CIRCUMFERENCE - length}`,
        dashOffset: -offset
      };

      offset += length;

      return slice;
    });

    return { key, title, subtitle, total, totalLabel, slices };
  }

}
