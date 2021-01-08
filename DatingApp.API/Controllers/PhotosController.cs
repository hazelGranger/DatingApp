using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Model.Model;
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
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig) 
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.Apikey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<ActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturn>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<ActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreateDto photoForCreateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
                
            var user = await _repo.GetUser(userId);

            var file = photoForCreateDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            photoForCreateDto.Url = uploadResult.Url.ToString();
            photoForCreateDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreateDto);

            if (!user.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            
            user.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                var PhotoToReturn = _mapper.Map<PhotoForReturn>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id}, PhotoToReturn);
            }

            return BadRequest("Could not add photo");

        }
    }
}