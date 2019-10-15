using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly iDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(iDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers() // metodo per restitutire una lista di utenti
        {
            var users = await _repo.GetUsers();

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id) // metodo per restituire un solo utente
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);   // user è la fonte, UserForDetailed è la destinazione
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]                       // viene usato per aggiornare risorse nella API
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)      // il modo per vedere se l'utente è colui che ha inviato il token al server
         {
             if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var userFromRepo = await _repo.GetUser(id);

             _mapper.Map(userForUpdateDto, userFromRepo);

             if (await _repo.SaveAll())     // se la modfica è andata a buon fine allora ritorna NoContent
             return NoContent();

             throw new Exception($"Updating user {id} failed on save");
         }
    }
}