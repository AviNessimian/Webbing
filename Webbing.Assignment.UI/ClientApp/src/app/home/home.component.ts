import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { UsageService } from '../services/usage.service';
import { UsageByCustomerDTO, UsageBySimDTO } from '../models/Usage';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  usageByCustomer: UsageByCustomerDTO[] = [];
  topCustomers: UsageByCustomerDTO[] = [];
  simsCount: number = 0;
  totalUsage: number = 0;

  fromDate: Date = new Date(2023, 1, 1);
  toDate: Date = new Date(2025, 1, 1);

  constructor(
    private router: Router, 
    private readonly usageService: UsageService) { }

  ngOnInit(): void {
    this._init();
  }

  onTopCustomerClick(customer: UsageByCustomerDTO) {
    this.router.navigate(['/usage-data', customer.customerId]);
  }

  onRefresh() {
    this._init();
  }

  private _init() {
    this.usageService.getUsagesGroupByCustomer(this.fromDate, this.toDate)
      .subscribe(data => {
        let sum = 0;
        let totalUsage = 0;

        data.forEach((el) => {
          sum += el.simCount;
          totalUsage += el.totalUsage;
        });

        this.simsCount = sum;
        this.totalUsage = totalUsage;
        this.usageByCustomer = data;
        this.topCustomers = data.sort((a, b) => b.totalUsage - a.totalUsage).slice(0, 2);
      });
  }
}
