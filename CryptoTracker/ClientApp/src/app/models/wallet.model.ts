import { IDocument } from "./document.model";

export interface IWallet extends IDocument {
    userId?:string;
    invested?: IInvestedAmountWallet[]
    coins?: ICoinInfoWallet[];
}

export interface IInvestedAmountWallet {
    fiat?:string;
    value?:number;
    source?:string;
}

export interface ICoinInfoWallet{
    symbol?: string;
    averagePrice?: number;
    amount?: number;
}