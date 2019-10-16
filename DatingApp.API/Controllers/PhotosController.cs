using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly iDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(iDatingRepository repo, IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account (
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]      // serve per aggiungere foto
        public async Task<IActionResult> AddPhotoForUser(int userId,
            [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // serve per vedere se siamo autorizzati
             return Unauthorized();

             var userFromRepo = await _repo.GetUser(userId);

             var file = photoForCreationDto.File;

             var uploadResult = new ImageUploadResult();       // archiviamo il risultato che ci da il cloud

             if (file.Length > 0)
             {
                 using (var stream = file.OpenReadStream())
                 {
                     var uploadParams = new ImageUploadParams()
                     {
                         File = new FileDescription(file.Name, stream),
                         Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                     };

                     uploadResult = _cloudinary.Upload(uploadParams);
                 }
             }

             photoForCreationDto.Url = uploadResult.Uri.ToString();
             photoForCreationDto.PublicId = uploadResult.PublicId;

             var photo = _mapper.Map<Photo>(photoForCreationDto);

             if (!userFromRepo.Photos.Any(u => u.IsMain))       // se questo da FALSE vuol dire che l'utente non ha una foto principale
                photo.IsMain = true;

                userFromRepo.Photos.Add(photo);

                if (await _repo.SaveAll())
                {
                    var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                    return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
                }

                return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")] // serve per impostare una foto come principale
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var user = await _repo.GetUser(userId);

             if (!user.Photos.Any(p => p.Id == id)) // se l'id che passiamo non corrisponde a nessun id delle foto in galleria
                return Unauthorized();

                var photoFromRepo = await _repo.GetPhoto(id);

                if (photoFromRepo.IsMain) // serve per vedere se la foto è gia in main
                    return BadRequest("This is already the main photo");

                    var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
                    currentMainPhoto.IsMain = false;

                    photoFromRepo.IsMain = true;

                    if (await _repo.SaveAll())
                        return NoContent();

                    return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")] // metodo per eliminare le foto dal profilo
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var user = await _repo.GetUser(userId);

             if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain) // serve per vedere se la foto è gia in main
                return BadRequest("You cannot delete your main photo");

            if (photoFromRepo.PublicId != null) // se la foto si trova su cloudinary e quindi ho una PublicId esegue questo altrimenti quello dopo
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok") 
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }

}
}
