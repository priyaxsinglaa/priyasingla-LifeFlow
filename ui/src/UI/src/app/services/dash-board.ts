import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
export interface BloodStockDto {
  bloodType: string;
  availableUnits: number;
  predictedDemand: number;
  supplyLevel: number;
  status: string;
}
export interface AlertResponseDto {
  id: number;
  severity: string;
  bloodType: string;
  hospital: string;
  units: number;
  isActive: boolean;
  createdDate: string;
}

export interface CreateDonationDto {
  bloodType: string;
  units: number;
  donationDate: string;
  donorName?: string;
  contact?: string;
  hospital: string;
  notes?: string;
}

export interface DonationResponseDto {
  id: number;
  bloodType: string;
  units: number;
  donationDate: string;
  donorName?: string;
  contact?: string;
  hospital: string;
  notes?: string;
}

export interface PaginatedDonationResponse {
  data: DonationResponseDto[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ForecastRequestDto {
  bloodType: string;
  hospital: string;
  daysAhead: number;
}

export interface DailyForecast {
  date: string;
  predictedUnits: number;
  lowerBound: number;
  upperBound: number;
}

export interface ForecastResponseDto {
  bloodType: string;
  hospital: string;
  predictions: DailyForecast[];
  aiInsight?: string;
}

export interface ReportResponseDto {
  fromDate: string;
  toDate: string;
  totalDonations: number;
  totalUnits: number;
  donations: any[]; // Maps to DonationResponseDto
  alerts: any[];    // Maps to AlertResponseDto
  unitsByBloodType: { [key: string]: number };
  donationsByHospital: { [key: string]: number };
}
@Injectable({
  providedIn: 'root'
})
export class dashBoardService {
  private readonly apiUrl = 'http://localhost:5187/api'; // Adjust port matching your local .NET App launch settings

  constructor(private http: HttpClient) {}


  getBloodStocks(): Observable<BloodStockDto[]> {
    return this.http.get<BloodStockDto[]>(`${this.apiUrl}/bloodstock`);
  }

  getActiveAlerts(): Observable<AlertResponseDto[]> {
    return this.http.get<AlertResponseDto[]>(`${this.apiUrl}/alerts`);
  }
  evaluateAlerts(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/alerts/evaluate`, {});
  }
  resolveAlert(id: number): Observable<AlertResponseDto> {
  return this.http.patch<AlertResponseDto>(`${this.apiUrl}/alerts/${id}/resolve`, {});
  }
  getDonations(
    bloodType?: string,
    hospital?: string,
    from?: string,
    to?: string,
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedDonationResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (bloodType && bloodType !== 'All') params = params.set('bloodType', bloodType);
    if (hospital) params = params.set('hospital', hospital);
    if (from) params = params.set('from', from);
    if (to) params = params.set('to', to);

    return this.http.get<PaginatedDonationResponse>(`${this.apiUrl}/donations`, { params });
  }

  createDonation(dto: CreateDonationDto): Observable<DonationResponseDto> {
    return this.http.post<DonationResponseDto>(`${this.apiUrl}/donations`, dto);
  }

  deleteDonation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/donations/${id}`);
  }
  
  getBloodTypes(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/forecast/blood-types`);
  }

  runPrediction(dto: ForecastRequestDto): Observable<ForecastResponseDto> {
    return this.http.post<ForecastResponseDto>(`${this.apiUrl}/forecast`, dto);
  }

  getHospitals(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/reports/hospitals`);
  }

  getReportData(from: string, to: string, hospital?: string): Observable<ReportResponseDto> {
    let params = new HttpParams()
      .set('from', from)
      .set('to', to);
    
    if (hospital && hospital.trim() !== '') {
      params = params.set('hospital', hospital);
    }

    return this.http.get<ReportResponseDto>(`${this.apiUrl}/reports`, { params });
  }

  exportExcel(from: string, to: string, hospital?: string): Observable<Blob> {
    let params = new HttpParams()
      .set('from', from)
      .set('to', to);
    
    if (hospital && hospital.trim() !== '') {
      params = params.set('hospital', hospital);
    }

    return this.http.get(`${this.apiUrl}/reports/export`, {
      params,
      responseType: 'blob'
    });
  }
}
