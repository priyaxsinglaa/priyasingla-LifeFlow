import { Component, OnInit,signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { dashBoardService, CreateDonationDto,DonationResponseDto} from '../../services/dash-board';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-donations',
  standalone: true,
  templateUrl: 'donation.html',
  
  imports: [CommonModule, FormsModule],
  styleUrls: ['donation.css']
})
export class DonationsComponent implements OnInit {
  donationsList = signal<DonationResponseDto[]>([]);
  showFormPanel = signal<boolean>(false);
  
  // Pagination State Signals
  totalCount = signal<number>(0);
  currentPage = signal<number>(1);
  totalPages = signal<number>(1);
  pageSize = 10;

  // Filter Form State Definitions
  filterBloodType = '';
  filterFromDate = '';
  filterToDate = '';

  // Input Collection Form Model Architecture
  formModel: CreateDonationDto = this.getEmptyFormModel();

  constructor(private donationService: dashBoardService) {}

  ngOnInit(): void {
    this.loadDonations();
  }

  loadDonations(): void {
    this.donationService.getDonations(
      this.filterBloodType,
      undefined, // Optional hospital query filter variable field
      this.filterFromDate,
      this.filterToDate,
      this.currentPage(),
      this.pageSize
    ).subscribe({
      next: (response) => {
        this.donationsList.set(response.data);
        this.totalCount.set(response.total);
        this.totalPages.set(response.totalPages);
      },
      error: (err) => console.error('Error fetching donations list:', err)
    });
  }

  applyFilters(): void {
    this.currentPage.set(1);
    this.loadDonations();
  }

  clearFilters(): void {
    this.filterBloodType = '';
    this.filterFromDate = '';
    this.filterToDate = '';
    this.currentPage.set(1);
    this.loadDonations();
  }

  toggleFormPanel(): void {
    this.showFormPanel.set(!this.showFormPanel());
    if (!this.showFormPanel()) {
      this.formModel = this.getEmptyFormModel();
    }
  }

  submitDonation(): void {
    this.donationService.createDonation(this.formModel).subscribe({
      next: () => {
        this.loadDonations();
        this.toggleFormPanel();
      },
      error: (err) => console.error('Failed to register record input parameters:', err)
    });
  }

  deleteRecord(id: number): void {
    if (confirm('Are you sure you want to permanently delete this entry?')) {
      this.donationService.deleteDonation(id).subscribe({
        next: () => this.loadDonations()
      });
    }
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadDonations();
    }
  }

  private getEmptyFormModel(): CreateDonationDto {
    return {
      bloodType: '',
      units: 1,
      donationDate: new Date().toISOString().substring(0, 10),
      donorName: '',
      contact: '',
      hospital: '',
      notes: ''
    };
}}