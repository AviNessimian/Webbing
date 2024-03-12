export interface UsageByCustomerDTO {
    customerId: string;
    customerName: string;
    simCount: number;
    totalUsage: number;
    lastUpdateInDays: number;
}

export interface UsageBySimDTO {
    simId: string;
    totalUsage: number;
}