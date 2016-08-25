using Microsoft.AspNetCore.Mvc;

namespace FALKR_SubstitutionCipher_v2.Controllers
{
    public class EncryptController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Encrypt() 
        {
            return View("Index");
        }

    }
}
