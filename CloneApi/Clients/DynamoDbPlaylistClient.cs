
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
    public class DynamoDbPlaylistClient : IDynamoDbPlaylistClient, IDisposable
    {
        public string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbPlaylistClient(IAmazonDynamoDB dynamoDB)
        {
            _dynamoDb = dynamoDB;
            _tableName = "UsersPlaylists";
        }

        public async Task<Playlist> GetData(string Id)
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

            var result = response.Item.ToClass<Playlist>();
            result.VideoIds = new List<string>(response.Item["VideoIds"].SS);
            result.VideoTitles = new List<string>(response.Item["VideoTitles"].SS);

            return result;

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

        public async Task<bool> PostPlaylist(Playlist data)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue(s: data.Id)},
                    {"PlaylistName", new AttributeValue(s: data.PlaylistName)},

                    {"VideoIds", new AttributeValue(data.VideoIds) },
                    {"VideoTitles", new AttributeValue(data.VideoTitles) },

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

        public async Task<bool> DelateVideoFromList(string Id, string videoId)
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
                return false;
            }

            var data = response.Item.ToClass<Playlist>();

            data.VideoIds = new List<string>(response.Item["VideoIds"].SS);
            data.VideoTitles = new List<string>(response.Item["VideoTitles"].SS);


            data.VideoTitles.RemoveAt(data.VideoIds.IndexOf(videoId));
            data.VideoIds.Remove(videoId);

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue(s: data.Id)},
                    {"PlaylistName", new AttributeValue(s: data.PlaylistName)},

                    {"VideoIds", new AttributeValue(data.VideoIds) },
                    {"VideoTitles", new AttributeValue(data.VideoTitles) },

                    {"UserName", new AttributeValue(s: data.UserName)},
                    {"UserId", new AttributeValue(s: data.UserId)}

                }
            };

            try
            {
                var response2 = await _dynamoDb.PutItemAsync(request);
                return response2.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine("Here is tour error\n" + e);
                return false;

            }
        }

        public async Task<bool> PostVideos(Playlist playlist)
        {
            var item = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue(playlist.Id) }
                    }
            };
            var response = await _dynamoDb.GetItemAsync(item);


            if (response == null || response.Item.Count == 0)
            {
                return false;
            }

            var data = response.Item.ToClass<Playlist>();

            data.VideoIds = new List<string>(response.Item["VideoIds"].SS);
            data.VideoTitles = new List<string>(response.Item["VideoTitles"].SS);

            for (int i = 0; i < playlist.VideoIds.Count; i++)
            {
                data.VideoIds.Add(playlist.VideoIds[i]);
                data.VideoTitles.Add(playlist.VideoTitles[i]);
            }


            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue(s: data.Id)},
                    {"PlaylistName", new AttributeValue(s: data.PlaylistName)},

                    {"VideoIds", new AttributeValue(data.VideoIds) },
                    {"VideoTitles", new AttributeValue(data.VideoTitles) },

                    {"UserName", new AttributeValue(s: data.UserName)},
                    {"UserId", new AttributeValue(s: data.UserId)}

                }
            };

            try
            {
                var response2 = await _dynamoDb.PutItemAsync(request);
                return response2.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine("Here is tour error\n" + e);
                return false;

            }

        }

        public async Task<List<Playlist>> GetAll()
        {

            var result = new List<Playlist> { };
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
                result.Add(item.ToClass<Playlist>());
                result[response.Items.IndexOf(item)].VideoIds = new List<string>(item["VideoIds"].SS);
                result[response.Items.IndexOf(item)].VideoTitles = new List<string>(item["VideoTitles"].SS);
            }


            return result;

        }

        public void Dispose()
        {
            _dynamoDb.Dispose();
        }

        public async Task<bool> DelatePlayList(string Id)
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
                var response = await _dynamoDb.DeleteItemAsync(item);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine("Here is tour error\n" + e);
                return false;

            }

        }

        public async Task<List<Playlist>> GetUSerPlaylists(string userId)
        {
            var result = new List<Playlist> { };
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
                var it = item.ToClass<Playlist>();
                if (it.UserId == userId)
                {
                    it.VideoIds = new List<string>(item["VideoIds"].SS);
                    it.VideoTitles = new List<string>(item["VideoTitles"].SS);
                    result.Add(it);
                }

            }

            return result;
        }
    }
}
