import { Component, Inject, Input } from '@angular/core';
import { UsageBySimDTO } from '../models/Usage';
import { UsageService } from '../services/usage.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-usage-data',
  templateUrl: './usage-data.component.html'
})
export class UsageDataComponent {
  usageBySim: UsageBySimDTO[] = [];
  customerId!: string;
  fromDate: Date = new Date(2023, 1, 1);
  toDate: Date = new Date(2025, 1, 1);

  constructor(
    private route: ActivatedRoute,
    private readonly usageService: UsageService) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.customerId = params['id'];
      this.usageService.getUsagesGroupBySim(this.customerId, this.fromDate, this.toDate)
        .subscribe(data => {
          this.usageBySim = data;
        });
    });

  }

}
