
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloneApi.Models;
using Newtonsoft.Json;



using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.IO;
using System.Threading;
using System.Reflection;
using CloneApi.News;

namespace CloneApi.Clients
{
    public class YouTubeApiClient : IYouTubeApiClient
    {
        private readonly YouTubeService _youTubeService;
        //private readonly YouTubeService _youTubeServiceUser;


        public YouTubeApiClient(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
            //_youTubeServiceUser
        }


        public async Task<List<Models.Video>> GetSerchByArtist(string artist)
        {
            var searchListRequest = _youTubeService.Search.List("id,snippet");
            searchListRequest.Q = artist; // Replace with your search term.
            searchListRequest.MaxResults = 50;



            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            string ChannelID = "";

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#channel")
                {
                    ChannelID = searchResult.Id.ChannelId;
                    break;
                }
            }


            var searchChunnelUploads = _youTubeService.Channels.List("contentDetails");
            searchChunnelUploads.Id = ChannelID;

            var searchChunnelUploadsResponse = await searchChunnelUploads.ExecuteAsync();
            Channel Channel = null;
            if (searchChunnelUploadsResponse.Items != null && searchChunnelUploadsResponse.Items.Count != 0)
            {
                Channel = searchChunnelUploadsResponse.Items[0];
            }
            else
            {
                return new List<Models.Video> { };
            }
            string UploardPlayListId = Channel.ContentDetails.RelatedPlaylists.Uploads;


            var searchUploadsVideos = _youTubeService.PlaylistItems.List("id,snippet,contentDetails");
            searchUploadsVideos.PlaylistId = UploardPlayListId;
            searchUploadsVideos.MaxResults = 50;


            var searchUploadsVideosResponse = await searchUploadsVideos.ExecuteAsync();

            List<string> VideosForSortIds = new List<string>();

            foreach (var searchResult in searchUploadsVideosResponse.Items)
            {
                VideosForSortIds.Add(searchResult.ContentDetails.VideoId);
            }

            for (int i = 0; i < 4; i++)
            {
                if (searchUploadsVideosResponse.NextPageToken != null)
                {
                    searchUploadsVideos.PageToken = searchUploadsVideosResponse.NextPageToken;
                    searchUploadsVideosResponse = await searchUploadsVideos.ExecuteAsync();

                    foreach (var searchResult in searchUploadsVideosResponse.Items)
                    {
                        VideosForSortIds.Add(searchResult.ContentDetails.VideoId);
                    }
                }
                else break;
            }
            List<VideosForSort> SortedBestVideos = new List<VideosForSort> { };



            if (VideosForSortIds.Count < 50)
            {
                var VideosStats = _youTubeService.Videos.List("id, statistics,snippet");
                VideosStats.Id = VideosForSortIds;
                VideosStats.MaxResults = VideosForSortIds.Count;
                var VideosStatsResponse = await VideosStats.ExecuteAsync();

                foreach (var searchResult in VideosStatsResponse.Items)
                {
                    SortedBestVideos.Add(new VideosForSort
                    {
                        VideoId = searchResult.Id,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle == null ? "" : searchResult.Snippet.ChannelTitle,
                        Vievs = searchResult.Statistics.ViewCount.Value
                    });

                }
            }
            else
            {
                for (int i = 0; i < VideosForSortIds.Count / 50.0; i++)
                {

                    List<string> HelpVideoIds = new List<string> { };
                    for (int j = i * 50; j < (i * 50) + 50; j++)
                    {
                        if (j < VideosForSortIds.Count)
                        {
                            HelpVideoIds.Add(VideosForSortIds[j]);
                        }
                    }
                    var VideosStats = _youTubeService.Videos.List("id, statistics,snippet");
                    VideosStats.Id = HelpVideoIds;
                    VideosStats.MaxResults = HelpVideoIds.Count;

                    var VideosStatsResponse = await VideosStats.ExecuteAsync();

                    foreach (var searchResult in VideosStatsResponse.Items)
                    {
                        SortedBestVideos.Add(new VideosForSort
                        {
                            VideoId = searchResult.Id,
                            VideoTitle = searchResult.Snippet.Title,
                            ChannelTitle = searchResult.Snippet.ChannelTitle == null ? "" : searchResult.Snippet.ChannelTitle,
                            Vievs = searchResult.Statistics.ViewCount.Value
                        });

                    }
                }
            }
            var Best = SortedBestVideos.OrderByDescending(x => x.Vievs);

            List<Models.Video> result = new List<Models.Video>();

            foreach (var video in Best)
            {
                result.Add(new Models.Video
                {
                    VideoId = video.VideoId,
                    VideoTitle = video.VideoTitle,
                    ChannelTitle = video.ChannelTitle
                });
            }

            return result;

        }


        public async Task<List<Models.Video>> GetSerchByVideoRequest(string request)
        {
            var searchListRequest = _youTubeService.Search.List("id,snippet");
            searchListRequest.Q = request; // Replace with your search term.
            searchListRequest.MaxResults = 5;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<Models.Video> result = new List<Models.Video> { };

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    result.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }
            return result;
        }

        public async Task<List<Models.Video>> GetTrendingMusic()
        {
            try
            {
                new OriginalPlaylist().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            var searchListRequest = _youTubeService.Search.List("id,snippet");
            searchListRequest.Q = ""; // Replace with your search term.
            searchListRequest.MaxResults = 10;
            searchListRequest.TopicId = "/m/04rlf";
            searchListRequest.PublishedAfter = DateTime.Today;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<Models.Video> result = new List<Models.Video> { };

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    result.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }
            return result;
        }

        //0	Classical music 1Pop music · 2Country · 3Reggae · 4	Rock music · 5	Jazz·  6Hip hop music 7Electronic music


        public async Task<List<List<Models.Video>>> GetSerchByGenres()
        {



            
            List<List<Models.Video>> result = new List<List<Models.Video>> {  };

            var searchListRequest = _youTubeService.Search.List("id,snippet");
            searchListRequest.Q = "Classical music"; // Replace with your search term.
            searchListRequest.MaxResults = 10;

            searchListRequest.TopicId = "/ m / 0ggq0m";//Classical music

            var searchListResponseClassical = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultClassical = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseClassical.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultClassical.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }

            searchListRequest.Q = "Pop music"; // Replace with your search term.


            searchListRequest.TopicId = "/m/064t9";//Pop music


            var searchListResponsePop = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultPop = new List<Models.Video> { };

            foreach (var searchResult in searchListResponsePop.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultPop.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }
            searchListRequest.Q = "Country music"; // Replace with your search term.


            searchListRequest.TopicId = "/m/01lyv";//Country

            var searchListResponseCountry = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultCountry = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseCountry.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultCountry.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }

            searchListRequest.Q = "Reggae music"; // Replace with your search term.

            searchListRequest.TopicId = "/m/06cqb";//Reggae

            var searchListResponseReggae = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultReggae = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseReggae.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultReggae.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }


            searchListRequest.Q = "Rock music"; // Replace with your search term.

            searchListRequest.TopicId = "/m/06by7";//Rock music

            var searchListResponseRock = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultRock = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseRock.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultRock.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }

            searchListRequest.Q = "Jazz music"; // Replace with your search term.

            searchListRequest.TopicId = "/m/03_d0";//	Jazz

            var searchListResponseJazz = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultJazz = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseJazz.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultJazz.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }
            searchListRequest.Q = "Hip hop music"; // Replace with your search term.

            searchListRequest.TopicId = "/m/0glt670";//Hip hop music

            var searchListResponseHip = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultHip = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseHip.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultHip.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }

            searchListRequest.Q = "Electronic music"; // Replace with your search term.

            searchListRequest.TopicId = "/m/02lkt";//Electronic music

            var searchListResponseElectronic = await searchListRequest.ExecuteAsync();

            List<Models.Video> resultElectronic = new List<Models.Video> { };

            foreach (var searchResult in searchListResponseElectronic.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    resultElectronic.Add(new Models.Video
                    {
                        VideoId = searchResult.Id.VideoId,
                        VideoTitle = searchResult.Snippet.Title,
                        ChannelTitle = searchResult.Snippet.ChannelTitle
                    });
                }
            }

            result.Add(resultClassical);
            result.Add(resultPop);
            result.Add(resultCountry);
            result.Add(resultReggae);
            result.Add(resultRock);
            result.Add(resultJazz);
            result.Add(resultHip);
            result.Add(resultElectronic);

                return result;



            //using (StreamReader stream = File.OpenText(@"D:\Code\C#\ynik\UserBotReuestApi\UserBotReuestApi\Clients\music.txt"))
            //{

            //    List<List<Models.Video>> result = JsonConvert.DeserializeObject<List<List<Models.Video>>>(stream.ReadToEnd());

            //    return result;

            //}

        }

        public async Task<List<LikeVideo>> GetVideoInfo(string userId, string userName, List<string> VideoIds)
        {
            var VideosStats = _youTubeService.Videos.List("id, snippet");
            VideosStats.Id = VideoIds;
            VideosStats.MaxResults = VideoIds.Count;
            var VideosStatsResponse = await VideosStats.ExecuteAsync();


            List<LikeVideo> result = new List<LikeVideo> { };

            foreach (var searchResult in VideosStatsResponse.Items)
            {
                result.Add(new LikeVideo
                {
                    UserId = userId,
                    UserName = userName,
                    VideolId = searchResult.Id,
                    VideoTitle = searchResult.Snippet.Title,
                    ChannelId = searchResult.Snippet.ChannelId,
                    ChannelTitle = searchResult.Snippet.ChannelTitle == null ? "" : searchResult.Snippet.ChannelTitle

                });

            }
            return result;
        }




        //public async Task<Models.Playlist> PostPlaylist(string playlistName, List<Models.Video> videos)
        //{

        //    UserCredential credential;

        //    using (var stream = new FileStream(@"D:\Code\C#\ynik\UserBotReuestApi\UserBotReuestApi\Clients\client_secret_328706793984-i7dldi3k900ns9m76dprbg57ihik0h8c.apps.googleusercontent.com (5).json", FileMode.Open, FileAccess.Read))
        //    {
        //        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            clientSecrets: GoogleClientSecrets.FromStream(stream).Secrets,
        //            // This OAuth 2.0 access scope allows an application to upload files to the
        //            // authenticated user's YouTube channel, but doesn't allow other types of access.
        //            new[] { YouTubeService.Scope.YoutubeUpload },
        //            "Webclient2",
        //            CancellationToken.None
        //        );
        //    }
        //    var youtubeServiceForUser = new YouTubeService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
        //    });

        //    var newPlaylist = new Google.Apis.YouTube.v3.Data.Playlist();
        //    newPlaylist.Snippet = new PlaylistSnippet();
        //    newPlaylist.Snippet.Title = playlistName;
        //    newPlaylist.Status = new PlaylistStatus();
        //    newPlaylist.Status.PrivacyStatus = "public";
        //    try
        //    {
        //        newPlaylist = await youtubeServiceForUser.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Here is tour error\n" + e);
        //        return null;
        //    }
        //    // Add a video to the newly created playlist.

        //    foreach (var Addvideo in videos)
        //    {
        //        var newPlaylistItem = new PlaylistItem();
        //        newPlaylistItem.Snippet = new PlaylistItemSnippet();
        //        newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
        //        newPlaylistItem.Snippet.ResourceId = new ResourceId();
        //        newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
        //        newPlaylistItem.Snippet.ResourceId.VideoId = Addvideo.VideoId;
        //        try
        //        {
        //            newPlaylistItem = await youtubeServiceForUser.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Here is tour error\n" + e);
        //            return null;
        //        }
        //    }

        //    Models.Playlist playlistres = new Models.Playlist
        //    {
        //        PlaylistId = newPlaylist.Id,
        //        PlaylistName = newPlaylist.Snippet.Title,
        //    };

        //    return playlistres;

        //}


    }
}
