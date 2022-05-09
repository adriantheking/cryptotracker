import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

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
}