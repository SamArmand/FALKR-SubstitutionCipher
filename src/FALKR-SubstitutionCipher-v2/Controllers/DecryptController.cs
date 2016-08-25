using Microsoft.AspNetCore.Mvc;

namespace FALKR_SubstitutionCipher_v2.Controllers
{
    public class DecryptController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Decrypt()
        {
            return View("Index");
        }

    }
}
