import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ZondaBalanceModel } from "../models/zonda/ZondaBalances.model";

@Injectable({
    providedIn: 'root'
})
export class ZondaService{
    API_URL: string = "https://localhost:5001";
    constructor(private httpClient: HttpClient) {
        
    }

    public GetInvestedAmount():Observable<number>{
        return this.httpClient.get(this.API_URL+"/Zonda/GetInvestedAmount") as Observable<number>;
    }

    public GetWallets():Observable<ZondaBalanceModel[]>{
        return this.httpClient.get(this.API_URL+"/Zonda/GetWallets") as Observable<ZondaBalanceModel[]>;
    }
}