import { Component, OnInit, signal } from '@angular/core';
import { dashBoardService, AlertResponseDto} from '../../services/dash-board';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
@Component({
  selector: 'app-alerts',
  standalone: true,
  templateUrl: 'alerts.html',
  imports: [CommonModule, FormsModule],
  styleUrls: ['alerts.css']
})
export class AlertsComponent implements OnInit {
  activeAlertsList = signal<AlertResponseDto[]>([]);
  constructor(private service: dashBoardService) {}

  ngOnInit(): void {
    this.refreshAlerts();
  }

  refreshAlerts() {
    this.service.getActiveAlerts().subscribe(res => this.activeAlertsList.set(res));
  }

  triggerEvaluation() {
    this.service.evaluateAlerts().subscribe(res=> {
      this.refreshAlerts(); // Update the UI with the new alerts
    });
  }
  resolveAlert(id: number): void {
    this.service.resolveAlert(id).subscribe(() => {
      this.refreshAlerts(); // Seamlessly updates UI array state
    });
  }
}