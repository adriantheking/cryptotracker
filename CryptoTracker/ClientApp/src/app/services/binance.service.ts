import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})
export class ZondaService{
    
    constructor(private httpClient: HttpClient) {
        
    }

    public GetInvestedAmount():Observable<number>{
        return this.httpClient.get(environment.API_URL+"/Binance/GetInvestedAmount") as Observable<number>;
    }
}