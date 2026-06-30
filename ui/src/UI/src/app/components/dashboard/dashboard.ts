import { Component, OnInit, signal } from '@angular/core';
import { dashBoardService,BloodStockDto, AlertResponseDto } from '../../services/dash-board';
import { Router,RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: 'dashboard.html',
  
  imports: [CommonModule, FormsModule, RouterLink],
  styleUrls: ['dashboard.css']
})
export class DashboardComponent implements OnInit {
  bloodStocks = signal<BloodStockDto[]>([]); 
  activeAlerts = signal<AlertResponseDto[]>([]);
  currentDate = new Date(); 

  constructor(private service: dashBoardService, private router: Router) {}

  ngOnInit(): void {
    this.service.getBloodStocks().subscribe(res => {
      this.bloodStocks.set(res)
    });
    this.service.getActiveAlerts().subscribe(res => {
      this.activeAlerts.set(res.slice(0, 5))
    });
  }

  navigateToRecord() { this.router.navigate(['/donations'], { queryParams: { action: 'new' } })};
  navigateToForecast() { this.router.navigate(['/forecast']); }
  navigateToReports() { this.router.navigate(['/reports']); }
}