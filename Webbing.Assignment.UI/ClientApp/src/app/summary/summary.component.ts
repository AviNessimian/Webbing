import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UsageByCustomerDTO } from '../models/Usage';

@Component({
  selector: 'app-summary',
  template: `
        <div class="row"> 
            <div class="col mb-3">
                <button class="btn btn-primary" (click)="onRefresh()">Refresh Data</button>
            </div>
        </div>
        <ol class="list-group list-group-numbered">
            <li class="list-group-item d-flex justify-content-between align-items-start">
                <div class="ms-2 me-auto">
                <div class="fw-bold">Sims Count</div>
                Number of all sims...
                </div>
                <span class="badge bg-primary rounded-pill">{{ simsCount }}</span>
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-start">
                <div class="ms-2 me-auto">
                <div class="fw-bold">Total Usage</div>
                Total usage of all sims
                </div>
                <span class="badge bg-primary rounded-pill">{{ totalUsage }}MB</span>
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-start">
                <div class="ms-2 me-auto">
                <div class="fw-bold">Top {{ topCustomers.length }} Customer</div>

                <div class="list-group mt-2">
                    <a 
                    (click)="onTopCustomerClick(customer)" 
                    *ngFor="let customer of topCustomers; let i = index" 
                    class="list-group-item list-group-item-action" 
                    style="cursor: pointer"
                    [ngClass]="{ 'active': i === 0 }"
                    aria-current="true">
                        <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">{{ customer.customerName }}</h5>
                        <small>{{customer.lastUpdateInDays}} days ago</small>
                        </div>
                        <p class="mb-1">
                            SIMs count: {{ customer.simCount }} <br/>
                            Total Usage: {{ customer.totalUsage }}MB
                        </p>
                        <small>Customer Id: {{ customer.customerId }}</small>
                    </a>
                </div>
                
                </div>
                <span class="badge bg-primary rounded-pill">{{ topCustomers.length }}</span>
            </li>
        </ol>
  `
})
export class SummaryComponent {

  @Input() simsCount: number = 0;
  @Input() totalUsage: number = 0;
  @Input() topCustomers: UsageByCustomerDTO[] = [];

  @Output() topCustomerClick = new EventEmitter<UsageByCustomerDTO>();
  @Output() refresh = new EventEmitter();

  onTopCustomerClick(customer: UsageByCustomerDTO) {
    this.topCustomerClick.emit(customer);
  }

  onRefresh() {
    this.refresh.emit();
  }
}

