
using CloneApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneApi.Clients

{
    public interface IDynamoDbPlaylistClient
    {
        public Task<Playlist> GetData(string Id);
        public Task<bool> PostPlaylist(Playlist playlist);
        public Task<bool> PostVideos(Playlist playlist);

        public Task<bool> DelateVideoFromList(string Id, string videoId);
        public Task<bool> DelatePlayList(string Id);



        public Task<List<Playlist>> GetAll();
        public Task<string> GetLastId();
        public Task<List<Playlist>> GetUSerPlaylists(string userId);




    }
}


