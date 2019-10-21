using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]  // ogni volta che uno dei metodi qui sotto verrà chiamato, aggiornera la data di ultima attività
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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams) // metodo per restitutire una lista di utenti
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // prende l'id dal token

            var userFromRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender)) 
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male"; // serve per restiuire il sessp opposto
            }

            var users = await _repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
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

         [HttpPost("{id}/like/{recipientId}")]
         public async Task<IActionResult> LikeUser(int id, int recipientId) // recipient sono i destinatari
         {
             if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var like = await _repo.GetLike(id, recipientId);

             if (like != null)
                return BadRequest("Hai già apprezzato questo utente");


            if (await _repo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Impossibile apprezzare");

         }
    }
}