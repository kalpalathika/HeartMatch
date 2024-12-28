using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace API.DTOs;

public class RegisterDTO
{
    [Required]
    public required string Username {get; set;}
    [Required]
    public required string Password {get;set;}
}
