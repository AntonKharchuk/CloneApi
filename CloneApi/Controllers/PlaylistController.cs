

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

namespace CloneApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly ILogger<PlaylistController> _logger;
        private readonly IDynamoDbPlaylistClient _dynamoDbClient;

        public PlaylistController(ILogger<PlaylistController> logger, IDynamoDbPlaylistClient dynamoDbClient)
        {
            _logger = logger;
            _dynamoDbClient = dynamoDbClient;
        }

        [HttpGet("playlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlaylistById([FromQuery] string id)
        {
            var result = await _dynamoDbClient.GetData(id);

            if (result == null)
            {
                return NotFound("Obj not found");
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

        [HttpGet("userplaylists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetuserPlaylistById([FromQuery] string Userid)
        {
            var result = await _dynamoDbClient.GetUSerPlaylists(Userid);

            if (result == null || result.Count == 0)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var result = await _dynamoDbClient.GetAll();

            if (result == null || result.Count == 0)

            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }

        [HttpPost("addplaylist")]
        public async Task<IActionResult> PostPlaylist([FromBody] Playlist playlist)
        {


            var result = await _dynamoDbClient.PostPlaylist(playlist);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly added to DB");
        }

        [HttpPost("addvideos")]
        public async Task<IActionResult> PostVideosToPlaylist([FromBody] Playlist playlist)
        {


            var result = await _dynamoDbClient.PostVideos(playlist);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly added to DB");
        }
        [HttpDelete("deletevideos")]
        public async Task<IActionResult> DeleteVideoFromPlayList([FromQuery] string Id, string videoId)
        {

            var result = await _dynamoDbClient.DelateVideoFromList(Id, videoId);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Values have been successfuly deleted from DB");
        }


        [HttpDelete("deleteplaylist")]
        public async Task<IActionResult> DelatePlayList([FromQuery] string id)
        {


            var result = await _dynamoDbClient.DelatePlayList(id);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly deleted from DB");
        }

    }
}

