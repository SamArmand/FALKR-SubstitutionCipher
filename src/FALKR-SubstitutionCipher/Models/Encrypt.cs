using System.ComponentModel.DataAnnotations;

namespace FALKR_SubstitutionCipher.Models 
{

    public class Encrypt
    {

        public string Plaintext { get; set; }

        [Required]
        public string Key { get; set; }

        public string Ciphertext { get; set; }

    }

}

