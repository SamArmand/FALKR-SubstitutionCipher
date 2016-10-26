using System.ComponentModel.DataAnnotations;

public class EncryptModel
 {

    public string Plaintext { get; set; }

    [Required]
    public string Key { get; set; }

    public string Ciphertext { get; set; }

 }