import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { UsageByCustomerDTO, UsageBySimDTO } from "../models/Usage";

@Injectable({
    providedIn: 'root'
})
export class UsageService {
    private baseUrl = `${environment.baseUrl}/api`;

    constructor(private http: HttpClient) { }

    // Get usages grouped by SIM with query parameters
    getUsagesGroupBySim(customerId: string, fromDate: Date, toDate: Date): Observable<UsageBySimDTO[]> {
        const url = `${this.baseUrl}/usages-group-by-sim?customerId=${customerId}&fromDate=${fromDate.toISOString()}&toDate=${toDate.toISOString()}`;
        return this.http.get<UsageBySimDTO[]>(url)
            .pipe(
                catchError(this.handleError)
            );
    }

    // Get usages grouped by customer with query parameters
    getUsagesGroupByCustomer(fromDate: Date, toDate: Date): Observable<UsageByCustomerDTO[]> {
        const url = `${this.baseUrl}/usages-group-by-customer?fromDate=${fromDate.toISOString()}&toDate=${toDate.toISOString()}`;
        return this.http.get<UsageByCustomerDTO[]>(url)
            .pipe(
                catchError(this.handleError)
            );
    }

    private handleError(error: any) {
        let errorMessage: string;
        if (error.error instanceof ErrorEvent) {
            console.error('Client-side or network error occurred.');
            errorMessage = `An error occurred: ${error.error.message}`;
        } else {

            console.error('The backend returned an unsuccessful response code.');
            errorMessage = `Backend returned code ${error.status}: ${error.body.error}`;
        }
        return throwError(errorMessage);
    }

}