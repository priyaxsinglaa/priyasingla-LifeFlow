import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ForecastRequestDto,ForecastResponseDto,dashBoardService } from '../../services/dash-board';

@Component({
  selector: 'app-forecast',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './forecast.html',
  styleUrls: ['./forecast.css']
})
export class ForecastComponent implements OnInit {
  // Input parameters state bindings
  bloodTypesList = signal<string[]>([]);
  requestModel: ForecastRequestDto = {
    bloodType: '',
    hospital: '',
    daysAhead: 7
  };

  // Operational pipeline UI state handling
  isProcessing = signal<boolean>(false);
  forecastResult = signal<ForecastResponseDto | null>(null);

  constructor(private forecastService: dashBoardService) {}

  ngOnInit(): void {
    this.loadSystemBloodTypes();
  }

  loadSystemBloodTypes(): void {
    this.forecastService.getBloodTypes().subscribe({
      next: (types) => {
        this.bloodTypesList.set(types);
        if (types.length > 0) {
          this.requestModel.bloodType = types[0];
        }
      },
      error: (err) => {
        console.error('Could not load blood type variants lists', err);
        // Resilient fallback options arrays if the service configuration has issues
        this.bloodTypesList.set(['O+', 'O-', 'A+', 'A-', 'B+', 'B-', 'AB+', 'AB-']);
        this.requestModel.bloodType = 'O+';
      }
    });
  }

  executeForecastSimulation(): void {
    this.isProcessing.set(true);
    this.forecastResult.set(null);

    this.forecastService.runPrediction(this.requestModel).subscribe({
      next: (res) => {
        this.forecastResult.set(res);
        this.isProcessing.set(false);
      },
      error: (err) => {
        console.error('Ollama token calculation processing runtime failure:', err);
        this.isProcessing.set(false);
      }
    });
  }
}