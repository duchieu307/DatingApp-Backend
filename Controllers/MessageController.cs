using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microservices.Data;
using Microservices.Dtos;
using Microservices.Helpers;
using Microservices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/user/{userId}/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        public MessageController(IDatingRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet("{id}", Name = "GetMessage")]

        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repository.GetMessage(id);

            if (messageFromRepo == null) return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, 
            MessageForCreationDto messageForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userId;

            var recipient = await _repository.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null) return BadRequest("Không tìm thấy người dùng");

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repository.Add(message);

            if( await _repository.SaveAll())
                return CreatedAtRoute("GetMessage", new{id = message.Id, userId = userId}, message);
            
            throw new Exception("Không thể lưu tin nhắn");
        }
    }
}