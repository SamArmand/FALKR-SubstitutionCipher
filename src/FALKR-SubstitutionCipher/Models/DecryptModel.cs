using System.ComponentModel.DataAnnotations;

public class DecryptModel
 {
    [Required]
    public string Plaintext { get; set; }
    
    public string Key { get; set; }

    public string Ciphertext { get; set; }
    
    [Required]
    public string From { get; set; }

    [Required]
    public string To { get; set; }

 }