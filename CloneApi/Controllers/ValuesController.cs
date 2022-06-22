
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
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IDynamoDbClient _dynamoDbClient;




        public ValuesController(ILogger<ValuesController> logger, IDynamoDbClient dynamoDbClient)
        {
            _logger = logger;
            _dynamoDbClient = dynamoDbClient;
        }

        [HttpGet("item")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequestHistory([FromQuery] string id)
        {
            var result = await _dynamoDbClient.GetData(id);

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            var ResponseToUser = new UserRequest
            {
                Id = result.Id,
                UserId = result.UserId,
                Request = result.Request,
                Time = result.Time,
                UserName = result.UserName,
            };
            return Ok(ResponseToUser);
        }
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

            var ResponseToUser = result
                .Select(x => new UserRequest()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Request = x.Request,
                    Time = x.Time,
                    UserName = x.UserName,
                }
                ).ToList();


            return Ok(ResponseToUser);
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

        [HttpGet("lastmode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLastUserModee(string userId)
        {
            var result = await _dynamoDbClient.GetLastUserMode(userId);

            if (result == null)
            {
                return NotFound("Obj not found");
            }

            return Ok(result);
        }



        [HttpPost("add")]
        public async Task<IActionResult> PostRequestToHistory([FromBody] UserRequest userRequest)
        {
            //var data = new UserRequest
            //{

            //}

            var result = await _dynamoDbClient.PostDate(userRequest);

            if (result == false)
            {
                return BadRequest("Check console log");
            }

            return Ok("Value has been successfuly added to DB");
        }






    }
}
