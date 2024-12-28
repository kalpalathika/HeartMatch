
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController (DataContext context, ITokenService tokenService): BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username)) return BadRequest("Username is taken");
        using var hmac = new HMACSHA512(); // will be auto disposed
        
        var user  = new AppUser  // create a new object
        {
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user); // add it to the context User , the above object
        await context.SaveChangesAsync(); // save the changes to the db (await for it)

        return user;
    }
    [HttpPost("login")] 
    public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO) {
        var user = await context.Users.FirstOrDefaultAsync(x => 
        x.UserName == loginDTO.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for(int i= 0;i< computedHash.Length;i++){
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            
        }
        return new UserDto 
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserExists(string username) {
        return await context.Users.AnyAsync(x => x.UserName == username); // check if user exists
    }

    
}
