import { IDocument } from "./document.model";

export interface IWallet extends IDocument {
    userId?:string;
    invested?: IInvestedAmountWallet[]
}

export interface IInvestedAmountWallet {
    fiat?:string;
    value?:number;
    source?:string;
}