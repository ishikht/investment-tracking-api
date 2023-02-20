using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracking.Api.Controllers;

public class TransactionsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}