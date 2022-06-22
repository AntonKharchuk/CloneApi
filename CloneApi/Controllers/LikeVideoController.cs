

using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2;
using CloneApi.Extensions;
using CloneApi.Models;
using CloneApi.Clients;

namespace UserBotReuestApi.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class LikeVideoController : ControllerBase
    {
        private readonly ILogger<LikeVideoController> _logger;
        private readonly IDynamoDbVideoClient _dynamoDbClient;

        public LikeVideoController(ILogger<LikeVideoController> logger, IDynamoDbVideoClient dynamoDbClient)
        {
            _logger = logger;
            _dynamoDbClient = dynamoDbClient;
        }

        [HttpGet("likevideo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLikeByid([FromQuery] string id)
        {
            var result = await _dynamoDbClient.GetData(id);

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }

        [HttpGet("userlikevideos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLikeByUserId([FromQuery] string userId)
        {
            var result = await _dynamoDbClient.GetUserLikes(userId);

            if (result == null || result.Count == 0)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }

        [HttpGet("likemates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFriends([FromQuery] string userId)
        {
            var result = await _dynamoDbClient.GetLikeMates(userId);

            if (result == null || result.Count == 0)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllHistory()
        {
            var result = await _dynamoDbClient.GetAll();

            if (result == null)
            {
                return NotFound("No friends found");
            }

            return Ok(result);
        }
        [HttpGet("lastid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetlastId()
        {
            var result = await _dynamoDbClient.GetLastId();

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> PostRequestToHistory([FromBody] LikeVideo likeVideo)
        {

            var result = await _dynamoDbClient.PostDate(likeVideo);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly added to DB");
        }

        [HttpDelete("dell")]
        public async Task<IActionResult> DellLikeVideo([FromQuery] string userId, string VideoId)
        {

            var result = await _dynamoDbClient.DelateLikeVideo(userId, VideoId);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly removed from DB");
        }

        [HttpDelete("dellbyid")]
        public async Task<IActionResult> DellLikeVideoById([FromQuery] string Id)
        {

            var result = await _dynamoDbClient.DelateLikeVideoById(Id);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly removed from DB");
        }




    }
}

