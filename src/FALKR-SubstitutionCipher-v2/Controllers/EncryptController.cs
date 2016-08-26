using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace FALKR_SubstitutionCipher_v2.Controllers
{
    public class EncryptController : Controller
    {

        readonly char[] _alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

        private char Replace(char v, string key)
        {
            for (var i = 0; i < _alphabet.Length; i++)
                if (v == _alphabet.ElementAt(i))
                    return key.ElementAt(i);

            return v;
 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Encrypt(EncryptModel model)
        {
            if (ModelState.IsValid)
            {

                model.Key = model.Key.ToUpper();
                var key = model.Key;

                var alphabet = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

                var count = 27;

                for (var i=0; i<alphabet.GetLength(0); i++)
                {
                    if (!key.Where((t, j) => key.ElementAt(j) == alphabet[i]).Any()) continue;
                    count--;
                    alphabet[i] = '.';
                }

                var plaintext = model.Plaintext.ToUpper();

                var sb = new StringBuilder();

                for (var i = 0; i < plaintext.Length; i++)
                    sb.Append(Replace(plaintext.ElementAt(i), key));

                model.Ciphertext = sb.ToString();

                return View("Index", model);

            }

            return View("Index");

        }

    }
}