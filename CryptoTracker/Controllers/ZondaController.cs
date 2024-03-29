﻿using CryptoCommon.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.CryptoTracker.Zonda;

namespace CryptoTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZondaController : ControllerBase
    {
        private readonly IZondaService zondaService;

        public ZondaController(IZondaService zondaService)
        {
            this.zondaService = zondaService;
        }

        [HttpGet(nameof(GetZondaOperations))]
        public async Task<IActionResult> GetZondaOperations(ZondaGetOperationsRequestModel request)
        {
            var zondaOperations = await this.zondaService.GetOperationsAsync(types: request.Types, balanceCurrencies: request.BalanceCurrencies);

            return Ok(zondaOperations);
        }

        [HttpGet(nameof(GetInvestedAmount))]
        public async Task<IActionResult> GetInvestedAmount()
        {
            var investedAmount = await this.zondaService.GetInvestedAmountAsync();
            return Ok(investedAmount);
        }

        [HttpGet(nameof(GetCryptoBalance))]
        public async Task<IActionResult> GetCryptoBalance()
        {
            //TODO: REMOVE IT
            var wallets = await this.zondaService.GetWallets();
            return Ok(await this.zondaService.GetCryptoBalancesAsync(wallets.Select(x => x.Currency).ToArray()));
        }

        [HttpGet(nameof(GetWallets))]
        public async Task<IActionResult> GetWallets()
        {
            return Ok(await this.zondaService.GetWallets());
        }
    }
}
