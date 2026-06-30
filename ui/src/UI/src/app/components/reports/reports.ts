import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportResponseDto,dashBoardService } from '../../services/dash-board';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reports.html',
  styleUrls: ['reports.css']
})
export class ReportsComponent implements OnInit {
  // Filtering Parameter Signal State
  hospitalOptions = signal<string[]>([]);
  fromDate = signal<string>('');
  toDate = signal<string>('');
  selectedHospital = signal<string>('');

  // UI Flow Tracking
  isLoading = signal<boolean>(false);
  activeTab = signal<'donations' | 'records' | 'alerts'>('donations');
  reportData = signal<ReportResponseDto | null>(null);

  constructor(private reportService: dashBoardService) {}

  ngOnInit(): void {
    this.initializeDefaultDates();
    this.loadHospitalsDropdown();
    this.fetchGeneratedReport();
  }

  initializeDefaultDates(): void {
    const today = new Date();
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(today.getMonth() - 1);

    this.toDate.set(today.toISOString().split('T')[0]);
    this.fromDate.set(oneMonthAgo.toISOString().split('T')[0]);
  }

  loadHospitalsDropdown(): void {
    this.reportService.getHospitals().subscribe({
      next: (hospitals) => this.hospitalOptions.set(hospitals),
      error: (err) => console.error('Failed to resolve distinct hospital references', err)
    });
  }

  fetchGeneratedReport(): void {
    this.isLoading.set(true);
    this.reportService.getReportData(this.fromDate(), this.toDate(), this.selectedHospital()).subscribe({
      next: (data) => {
        this.reportData.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('API Report generation routing execution crash', err);
        this.isLoading.set(false);
      }
    });
  }

  downloadExcelReport(): void {
    this.reportService.exportExcel(this.fromDate(), this.toDate(), this.selectedHospital()).subscribe({
      next: (blob) => {
        const hospitalPart = this.selectedHospital() ? `_${this.selectedHospital().replace(/\s+/g, '')}` : '';
        const filename = `LifeFlow_Report${hospitalPart}_${this.fromDate().replace(/-/g, '')}_${this.toDate().replace(/-/g, '')}.xlsx`;
        
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Excel binary file payload generation stream crash', err)
    });
  }

  // Object key transformation loops helpers for the matrix breakdown maps
  getMapKeys(obj: any): string[] {
    return obj ? Object.keys(obj) : [];
  }
}