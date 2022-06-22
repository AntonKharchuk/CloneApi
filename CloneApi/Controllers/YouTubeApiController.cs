

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloneApi.Clients;
using CloneApi.Models;


namespace CloneApi.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class YouTubeApiController : ControllerBase
    {
        private readonly ILogger<YouTubeApiController> _logger;
        private readonly IYouTubeApiClient _youTubeApiClient;

        public YouTubeApiController(ILogger<YouTubeApiController> logger, IYouTubeApiClient youTubeApiClient)
        {
            _logger = logger;
            _youTubeApiClient = youTubeApiClient;
        }

        [HttpGet("videosbyrequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequest([FromQuery] string request)
        {

            var result = await _youTubeApiClient.GetSerchByVideoRequest(request);

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            //var ResponseToUser = new LikeVideo
            //{
            //    Id = result.Id,
            //    UserId = result.UserId,
            //    VideoId = result.VideoId,
            //    VideoTitle = result.VideoTitle,
            //    ChannelId = result.ChannelId,
            //    ChannelTitle = result.ChannelTitle,
            //    UserName = result.UserName,
            //};
            return Ok(result);
        }

        [HttpGet("videoinfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Getvideoinfo(string userId, string userName, string VideoId)
        {
            var result = await _youTubeApiClient.GetVideoInfo(userId, userName, new List<string> { VideoId });

            if (result == null || result.Count == 0)
            {
                return NotFound("Obj not found");
            }

            return Ok(result[0]);
        }
        [HttpGet("videosinfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Getvideosinfo([FromQuery] List<string> Ids)
        {
            var result = await _youTubeApiClient.GetVideoInfo("None", "None", Ids);

            if (result == null || result.Count == 0)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }

        [HttpGet("artistbyrequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArtistSongs([FromQuery] string artist)
        {

            var result = await _youTubeApiClient.GetSerchByArtist(artist);

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            //var ResponseToUser = new LikeVideo
            //{
            //    Id = result.Id,
            //    UserId = result.UserId,
            //    VideoId = result.VideoId,
            //    VideoTitle = result.VideoTitle,
            //    ChannelId = result.ChannelId,
            //    ChannelTitle = result.ChannelTitle,
            //    UserName = result.UserName,
            //};
            return Ok(result);
        }
        [HttpGet("genresbyrequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGenres()
        {

            var result = await _youTubeApiClient.GetSerchByGenres();

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            //var ResponseToUser = new LikeVideo
            //{
            //    Id = result.Id,
            //    UserId = result.UserId,
            //    VideoId = result.VideoId,
            //    VideoTitle = result.VideoTitle,
            //    ChannelId = result.ChannelId,
            //    ChannelTitle = result.ChannelTitle,
            //    UserName = result.UserName,
            //};
            return Ok(result);
        }

        [HttpGet("trendsbyrequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrends()
        {

            var result = await _youTubeApiClient.GetTrendingMusic();

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            //var ResponseToUser = new LikeVideo
            //{
            //    Id = result.Id,
            //    UserId = result.UserId,
            //    VideoId = result.VideoId,
            //    VideoTitle = result.VideoTitle,
            //    ChannelId = result.ChannelId,
            //    ChannelTitle = result.ChannelTitle,
            //    UserName = result.UserName,
            //};
            return Ok(result);
        }

        /*
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllHistory()
        {
            var result = await _dynamoDbClient.GetAll();

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            //var ResponseToUser = result
            //    .Select(x => new LikeVideo()
            //    {
            //        Id = x.Id,
            //        UserId = x.UserId,
            //        ChannelId = x.ChannelId,
            //        ChannelTitle = x.ChannelTitle,
            //        VideoId = x.VideoId,
            //        VideoTitle = x.VideoTitle,
            //        UserName = x.UserName,
            //    }
            //    ).ToList();


            return Ok(result);
        }
        */
        //    [HttpPost("addplaylist")]
        //    public async Task<IActionResult> PostRequestToHistory([FromBody] List<VideoToPlaylist> vs )
        //    {


        //        List<Video> videos = new List<Video> { };

        //        foreach (var item in vs)
        //        {
        //            videos.Add(new Video
        //            {
        //                VideoId = item.VideoId,
        //                VideoTitle = item.VideoTitle,
        //                ChannelTitle = item.ChannelTitle

        //            }
        //            ) ;

        //        }
        //        Console.WriteLine("asf");

        //        var result = await _youTubeApiClient.PostPlaylist(vs[0].PlaylistName,videos);
        //        if (result == null)
        //        {
        //            return BadRequest("Check console log");
        //        }

        //        return Ok(result);
        //    }

    }
}

