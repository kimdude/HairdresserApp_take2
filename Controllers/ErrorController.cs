
using Microsoft.AspNetCore.Mvc;

namespace HairdresserApp.Controllers;

//Controller to handle error views
public class ErrorController : Controller
{
    //Errors
    [Route("Error/{statusCode}")]
    public IActionResult StatusCodeHandler(int statusCode)
    {
        if(statusCode == 404)
        {
            ViewBag.ErrorMessage = "Sidan du söker kunde tyvärr inte hittas. Kontrollera länken.";
        }
        else
        {
            ViewBag.ErrorMessage = "Ett oväntat fel uppstod. Vänligen prova igen senare.";
        }

        return View("NotFound");
    }
}