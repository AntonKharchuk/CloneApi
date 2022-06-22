
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloneApi.Models;


namespace CloneApi.Clients
{
    public interface IYouTubeApiClient
    {
        //+
        public Task<List<Video>> GetSerchByVideoRequest(string request);

        //+
        public Task<List<LikeVideo>> GetVideoInfo(string userId, string userName, List<string> VideoIds);


        //+
        public Task<List<Video>> GetSerchByArtist(string artist);

        //+
        public Task<List<List<Video>>> GetSerchByGenres();

        //+
        public Task<List<Models.Video>> GetTrendingMusic();


        //public Task<List<Video>> GetPopularSongs();

        //public Task<Playlist> PostPlaylist(string playlistName, List<Video> videos);

        //public Task<bool> PostDate(LikeVideo likeVideo);

        //public Task<List<LikeVideo>> GetAll();

    }
}
