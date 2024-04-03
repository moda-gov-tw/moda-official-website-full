using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using static Utility.YouTube.GetApi;

namespace Utility.YouTube
{
    public class GetApi
    {
        public static BaesData baesData { get; set; }

        static List<string> languageItems = new List<string>() { "zh-TW", "en" };


        public static void GetVideo(ref YouTubeVideoModel youTubeVideoModel)
        {
            using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = baesData.apiKey,
            }))
            {
                var playlistListRequest = youtubeService.Search.List("snippet,status");
                playlistListRequest.Type = "viode";
                playlistListRequest.PageToken = youTubeVideoModel.nextToken;
                playlistListRequest.ChannelId = baesData.channelld;
                playlistListRequest.MaxResults = 50;
                var listdata = playlistListRequest.Execute();
                var playData = new List<YouTubePlaylistItemDetailModel>();
                foreach (var item in listdata.Items)
                {
                    playData.Add(new YouTubePlaylistItemDetailModel()
                    {

                        YoutubeId = item.Id.VideoId,
                        Title = item.Snippet.Title,
                        Description = item.Snippet.Description,
                        PushData = item.Snippet.PublishedAt,
                    });
                }
                youTubeVideoModel.nextToken = listdata.NextPageToken;
                youTubeVideoModel.youTubePlaylistItemDetailModels.AddRange(playData);
                if (!string.IsNullOrWhiteSpace(youTubeVideoModel.nextToken))
                {
                    GetVideo(ref youTubeVideoModel);
                }
            }

        }
        public static List<YouTubePlayItemModel> GetPlayList()
        {
            var playData = new List<YouTubePlayItemModel>();
            foreach (var langer in languageItems)
            {
                using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = baesData.apiKey,
                }))
                {
                    var playlistListRequest = youtubeService.Playlists.List("snippet,status");
                    playlistListRequest.Key = baesData.apiKey;
                    playlistListRequest.ChannelId = baesData.channelld;
                    playlistListRequest.Hl = langer;
                    PlaylistListResponse listdata = playlistListRequest.Execute();
                    foreach (var item in listdata.Items.Where(X => X.Status.PrivacyStatus == "public"))
                    {
                        YouTubeLanguageTitleModel youTubeLanguageTitleModel = new YouTubeLanguageTitleModel()
                        {
                            Title = item.Snippet.Localized.Title,
                            Language = langer,
                            Description = item.Snippet.Localized.Description,
                        };

                        var youTubePlayItemModel = playData.FirstOrDefault(x => x.Id == item.Id);
                        if (youTubePlayItemModel != null)
                        {
                            youTubePlayItemModel.youTubeLanguageTitleModels.Add(youTubeLanguageTitleModel);
                        }
                        else
                        {
                            youTubePlayItemModel = new YouTubePlayItemModel()
                            {
                                Id = item.Id,
                                Title = item.Snippet.Title,
                                Description = item.Snippet.Description,
                            };
                            youTubePlayItemModel.youTubeLanguageTitleModels.Add(youTubeLanguageTitleModel);
                            playData.Add(youTubePlayItemModel);
                        }
                    }

                }
            }
            return playData;
        }

        public static void GetPlayListItems(ref YouTubePlayItemModel item, out string error, int sort = 0)
        {
            error = "";
            try
            {
                using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = baesData.apiKey,

                }))
                {
                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet,status");
                    playlistItemsListRequest.PlaylistId = item.Id;
                    playlistItemsListRequest.MaxResults = 50;

                    if (!string.IsNullOrWhiteSpace(item.nextToken))
                    {
                        playlistItemsListRequest.PageToken = item.nextToken;
                    }
                    PlaylistItemListResponse playlistItemsListResponse = playlistItemsListRequest.Execute();
                    item.nextToken = playlistItemsListResponse.NextPageToken;
                    var Detail = new List<YouTubePlaylistItemDetailModel>();
                    foreach (var playlistItem in playlistItemsListResponse.Items.Where(x => x.Status.PrivacyStatus == "public").OrderBy(x => x.Snippet.Position))
                    {
                        sort++;
                        var youTubePlaylistItemDetailModel = new YouTubePlaylistItemDetailModel()
                        {
                            YoutubeId = playlistItem.Snippet.ResourceId.VideoId,
                            Title = playlistItem.Snippet.Title,
                            Description = playlistItem.Snippet.Description,
                            PushData = playlistItem.Snippet.PublishedAt,
                            Sort = sort
                        };

                        GetVideo(ref youTubePlaylistItemDetailModel, out string err);
                        Detail.Add(youTubePlaylistItemDetailModel);
                    }
                    item.youTubePlaylistItemDetailModels.AddRange(Detail);
                    if (!string.IsNullOrWhiteSpace(item.nextToken))
                    {
                        GetPlayListItems(ref item, out string err, sort);
                        if (!string.IsNullOrWhiteSpace(err)) { error = err; };
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }
        }

        public static void GetVideo(ref YouTubePlaylistItemDetailModel item, out string error)
        {
            error = "";
            try
            {
                var defaultlanguage = "";
                foreach (var language in languageItems)
                {
                    using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        ApiKey = baesData.apiKey,

                    }))
                    {

                        var playlistItemsVideosRequest = youtubeService.Videos.List("snippet,status");
                        playlistItemsVideosRequest.Id = item.YoutubeId;
                        playlistItemsVideosRequest.Hl = language;
                        var playlistItemsVideosResponse = playlistItemsVideosRequest.Execute();
                        defaultlanguage = !string.IsNullOrWhiteSpace(playlistItemsVideosResponse.Items[0].Snippet.DefaultLanguage) ? playlistItemsVideosResponse.Items[0].Snippet.DefaultLanguage : playlistItemsVideosResponse.Items[0].Snippet.DefaultAudioLanguage;
                        item.Defaultlanguage = defaultlanguage;
                        var youTubeVideoLanguageTitleModel = new YouTubeLanguageTitleModel()
                        {
                            Title = playlistItemsVideosResponse.Items[0].Snippet.Localized.Title,
                            Description = playlistItemsVideosResponse.Items[0].Snippet.Localized.Description,
                            Language = language,
                        };
                        item.YouTubeLanguageTitleModels.Add(youTubeVideoLanguageTitleModel);
                    }
                }
                foreach (var langData in item.YouTubeLanguageTitleModels)
                {
                    if (defaultlanguage == langData.Language)
                    {
                        langData.HasData = true;
                    }
                    else
                    {
                        var defaultData = item.YouTubeLanguageTitleModels.First(x => x.Language == defaultlanguage);
                        if (langData.Title != defaultData.Title)
                        {
                            langData.HasData = true;
                        }
                        else
                        {
                            langData.HasData = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

        }

        public class BaesData
        {
            public string webSiteId { get; set; }

            public int webLevelMainSN { get; set; } = 0;
            public string apiKey { get; set; }
            public string channelld { get; set; }

            public string youtubeType { get; set; }
            public List<YouPageItemModel> youPageItemModels { get; set; } = new List<YouPageItemModel>();
        }
        public class YouPageItemModel
        {
            public string webLevelKey { get; set; }
            public string channelld { get; set; }
        }

        public class YouTubePlayItemModel
        {
            /// <summary>
            /// 播放列表Id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 簡介
            /// </summary>
            public string Description { get; set; }

            public string nextToken { get; set; }

            public string webLevelKey { get; set; }

            public int webLevelMainSN { get; set; }

            public string Defaultlanguage { get; set; }

            public List<YouTubeLanguageTitleModel> youTubeLanguageTitleModels { get; set; } = new List<YouTubeLanguageTitleModel>();

            public List<YouTubePlaylistItemDetailModel> youTubePlaylistItemDetailModels { get; set; } = new List<YouTubePlaylistItemDetailModel>();
        }

        public class YouTubePlaylistItemDetailModel
        {

            /// <summary>
            /// 影片代號
            /// </summary>
            public string YoutubeId { get; set; }
            /// <summary>
            /// 影片標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 影片說明
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 發布日期
            /// </summary>
            public DateTime? PushData { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            public int Sort { get; set; }

            /// <summary>
            /// 預設語系
            /// </summary>
            public string Defaultlanguage { get; set; }
            /// <summary>
            /// 多語系標題跟備註
            /// </summary>
            public List<YouTubeLanguageTitleModel> YouTubeLanguageTitleModels { get; set; } = new List<YouTubeLanguageTitleModel> { };

        }

        public class YouTubeLanguageTitleModel
        {
            public string Language { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public bool HasData { get; set; }
        }

        public class YouTubeVideoModel
        {
            /// <summary>
            /// 播放清單
            /// </summary>
            public string Title { get; set; }
            public string nextToken { get; set; }
            public List<YouTubePlaylistItemDetailModel> youTubePlaylistItemDetailModels { get; set; } = new List<YouTubePlaylistItemDetailModel>();
        }

    }
}
