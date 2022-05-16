export interface ZondaBalanceModel{
    id?:string;
    userId?: string;
    availableFunds?: number;
    totalFunds?: number;
    lockedFunds?: number;
    currency?: string;
    type?: string;
    name?: string;
    balanceEngine?: string;
}