using System.ComponentModel.DataAnnotations;

namespace FALKR_SubstitutionCipher.Models
{
    public class Decrypt
    {

        public string Plaintext { get; set; }
        
        public string Key { get; set; }
        
        [Required]
        public string Ciphertext { get; set; }
        
        public string From { get; set; }

        public string To { get; set; }

    }


}

