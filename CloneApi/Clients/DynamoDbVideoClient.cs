
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloneApi.Extensions;
using CloneApi.Models;

namespace CloneApi.Clients

{
    public class DynamoDbVideoClient : IDynamoDbVideoClient, IDisposable
    {
        public string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbVideoClient(IAmazonDynamoDB dynamoDB)
        {
            _dynamoDb = dynamoDB;
            _tableName = "LikedVideos";
        }

        public async Task<LikeVideo> GetData(string Id)
        {

            var item = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue(Id) }
                    }
            };
            var response = await _dynamoDb.GetItemAsync(item);


            if (response == null || response.Item.Count == 0)
            {
                return null;
            }

            var result = response.Item.ToClass<LikeVideo>();

            return result;

        }

        public async Task<List<LikeVideo>> GetAll()
        {

            var result = new List<LikeVideo> { };
            var request = new ScanRequest
            {
                TableName = _tableName
            };
            var response = await _dynamoDb.ScanAsync(request);

            if (response == null || response.Items.Count == 0)
            {
                return null;
            }

            foreach (var item in response.Items)
            {
                result.Add(item.ToClass<LikeVideo>());
            }


            return result;

        }

        public async Task<List<LikeVideo>> GetUserLikes(string userId)
        {

            var result = new List<LikeVideo> { };
            var request = new ScanRequest
            {
                TableName = _tableName
            };
            var response = await _dynamoDb.ScanAsync(request);

            if (response == null || response.Items.Count == 0)
            {
                return null;
            }

            foreach (var item in response.Items)
            {
                var it = item.ToClass<LikeVideo>();
                if (it.UserId == userId)
                {
                    result.Add(it);
                }
            }


            return result;

        }

        public async Task<List<List<LikeVideo>>> GetLikeMates(string userId)
        {
            var data = new List<LikeVideo> { };
            var request = new ScanRequest
            {
                TableName = _tableName
            };
            var response = await _dynamoDb.ScanAsync(request);

            if (response == null || response.Items.Count == 0)
            {
                return null;
            }

            foreach (var Likevid in response.Items)
            {
                data.Add(Likevid.ToClass<LikeVideo>());
            }

            var LoveChannels = data.Where(x => x.UserId == userId).Select(x => x.ChannelId);

            List<string> FriendIds = new List<string> { };

            //List<string> dd = new List<string> { };

            //foreach (var chan in LoveChannels)
            //{
            //    dd.Add(chan);
            //}

            foreach (var chan in LoveChannels)
            {
                foreach (var d in data)
                {
                    if (d.ChannelId == chan && d.UserId != userId && !FriendIds.Contains(d.UserId))
                    {
                        FriendIds.Add(d.UserId);
                    }
                }
            }

            var result = new List<List<LikeVideo>> { };

            foreach (var fId in FriendIds)
            {
                var friendLikes = new List<LikeVideo> { };

                foreach (var it in data)
                {
                    if (it.UserId == fId)
                    {
                        friendLikes.Add(it);
                    }
                }
                result.Add(friendLikes);
            }


            return result;

        }

        public async Task<bool> PostDate(LikeVideo data)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue(s: data.Id)},
                    {"ChannelId", new AttributeValue(s: data.ChannelId)},
                    {"ChannelTitle", new AttributeValue(s: data.ChannelTitle)},
                    {"VideolId", new AttributeValue(s: data.VideolId)},
                    {"VideoTitle", new AttributeValue(s: data.VideoTitle)},
                    {"UserName", new AttributeValue(s: data.UserName)},
                    {"UserId", new AttributeValue(s: data.UserId)}

                }
            };

            try
            {
                var response = await _dynamoDb.PutItemAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine("Here is tour error\n" + e);
                return false;

            }

        }


        public async Task<bool> DelateLikeVideo(string userId, string VideoId)
        {

            var data = new List<LikeVideo> { };
            var request = new ScanRequest
            {
                TableName = _tableName
            };
            var response = await _dynamoDb.ScanAsync(request);

            if (response == null || response.Items.Count == 0)
            {
                return false;
            }

            foreach (var Likevid in response.Items)
            {
                data.Add(Likevid.ToClass<LikeVideo>());
            }

            var WhatToDell = data.Where(x => x.UserId == userId && x.VideolId == VideoId).Select(x => x.Id);

            if (WhatToDell.Count() == 0)
            {
                return true;
            }

            DeleteItemResponse response2 = null;
            foreach (var Id in WhatToDell)
            {
                var item = new DeleteItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue(Id) }
                    }
                };

                try
                {
                    response2 = await _dynamoDb.DeleteItemAsync(item);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Here is tour error\n" + e);
                    return false;

                }
            }
            return response2.HttpStatusCode == System.Net.HttpStatusCode.OK;

        }

        public async Task<string> GetLastId()
        {
            var data = new List<UserRequest> { };
            var request = new ScanRequest
            {
                TableName = _tableName
            };
            var response = await _dynamoDb.ScanAsync(request);

            if (response == null || response.Items.Count == 0)
            {
                return null;
            }

            foreach (var item in response.Items)
            {
                data.Add(item.ToClass<UserRequest>());
            }

            string result = data.Max(x => int.Parse(x.Id)).ToString();

            return result;
        }

        public void Dispose()
        {
            _dynamoDb.Dispose();
        }

        public async Task<bool> DelateLikeVideoById(string videoId)
        {
            var item = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue(videoId) }
                    }
            };

            try
            {
                var response2 = await _dynamoDb.DeleteItemAsync(item);
                return response2.HttpStatusCode == System.Net.HttpStatusCode.OK;

            }
            catch (Exception e)
            {
                Console.WriteLine("Here is tour error\n" + e);
                return false;

            }
        }
    }
}

