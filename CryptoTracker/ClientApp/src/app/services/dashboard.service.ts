import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { IWallet } from "../models/wallet.model";

@Injectable({
    providedIn: 'root'
})
export class DashboardService{
    API_URL: string = "https://localhost:5001"
    constructor(private httpClient: HttpClient) {
        
    }

    public GetDashboard(): Observable<ZondaTransactionHistoryModel>{
        return this.httpClient.get(this.API_URL+'/Dashboard') as Observable<ZondaTransactionHistoryModel>;
    }

    public GetWallet():Observable<IWallet>{
        return this.httpClient.get(this.API_URL+'/Dashboard/GetWallet') as Observable<IWallet>;
    }
}

export interface ZondaTransactionHistoryModel{
    totalRows?: string;
    items?: ZondaTransactionHistoryItemModel[];
    nextPageCursor?: string;
}

export interface ZondaTransactionHistoryItemModel{
    id?: string;
    market?: string;
    time?: string;
    amount?: number;
    rate?: number;
    initializedBy?: string;
    wasTaker?: boolean;
    userAction?: string;
    offerId?: string;
    comissionValue?: number;
}