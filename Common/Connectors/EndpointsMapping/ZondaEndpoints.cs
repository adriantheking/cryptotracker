namespace Common.Connectors.EndpointsMapping
{
    public static class ZondaEndpoints
    {
        public static readonly string TransactionHistoryEndpoint = "/trading/history/transactions";
        public static readonly string OperationHistoryEndpoint = "/balances/BITBAY/history";
        public static readonly string WalletsList = "/balances/BITBAY/balance";
        public static readonly string Stats = "/trading/stats/";
    }
}
