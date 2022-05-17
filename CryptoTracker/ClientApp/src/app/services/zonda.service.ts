import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ZondaBalanceModel } from "../models/zonda/zondaBalances.model";
import { ZondaCryptoBalanceModel } from "../models/zonda/zondaCryptoBalance.model";

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

    public GetCryptoBalance():Observable<ZondaCryptoBalanceModel[]>{
        return this.httpClient.get(this.API_URL+"/Zonda/GetCryptoBalance") as Observable<ZondaCryptoBalanceModel[]>;
    }

    public GetWallets():Observable<ZondaBalanceModel[]>{
        return this.httpClient.get(this.API_URL+"/Zonda/GetWallets") as Observable<ZondaBalanceModel[]>;
    }
}