using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo; //mettere _ perchè è privata
        private readonly IConfiguration _config; //mettere _ perchè è privata
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]  //metodo registrazione
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {


            userForRegisterDto.Username = userForRegisterDto.Username.ToLower(); //permettiamo di accedere sia in maiuscolo e minuscolo

            if (await _repo.UserExists(userForRegisterDto.Username)) // vede se c'è gia un nome esistente
                return BadRequest("Username già esistente");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")] //metodo login
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password); //verifica che l'utente esista

            if (userFromRepo == null) // se il nome o password non esistono nel db restituisce null
                return Unauthorized(); // restituisco questo invece di 'nome giusto ma password errata' o viceversa

            var claims = new[] //se il login è avvenuto con succeso passo alla definizione del token mettenedo 2 parametri: Id e nome
            {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()), //Id scritto sul token
                    new Claim(ClaimTypes.Name, userFromRepo.Username) //nome scirtto sul token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor //token che viene creato
            {
                Subject = new ClaimsIdentity(claims), // il token viene creato inserendo l'iD e il nome
                Expires = DateTime.Now.AddDays(1), //tempo di scadenza del token
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

                return Ok(new
                {         // token rimandato ai nostri client
                    token = tokenHandler.WriteToken(token), 
                    user
                });




        }
    }
}