
using CloneApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneApi.Clients

{
    public interface IDynamoDbVideoClient
    {
        public Task<LikeVideo> GetData(string Id);
        public Task<List<LikeVideo>> GetAll();
        public Task<string> GetLastId();

        public Task<bool> PostDate(LikeVideo likeVideo);
        public Task<bool> DelateLikeVideo(string userId, string VideoId);
        public Task<bool> DelateLikeVideoById(string videoId);

        public Task<List<List<LikeVideo>>> GetLikeMates(string userId);
        public Task<List<LikeVideo>> GetUserLikes(string userId);







    }
}
