using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SerosMiniTwitchAPI
{
    public class SerosMiniTwitchAPI
    {
        public delegate void SMTAPIFollowChannelsHandler(SerosTwitchFollowModel model);
        public event SMTAPIFollowChannelsHandler GotFollowChannels;

        protected virtual void onGotFollowChannels(SerosTwitchFollowModel model)
        {
            GotFollowChannels?.Invoke(model);
        }

        public delegate void SMTAPIStreamDetailHandler(SerosTwitchFollowModelChannel channel, SerosTwitchFollowModel model);
        public event SMTAPIStreamDetailHandler GotStreamDetail;

        protected virtual void onGotStreamDetail(SerosTwitchFollowModelChannel channel, SerosTwitchFollowModel model)
        {
            GotStreamDetail?.Invoke(channel, model);
        }

        public delegate void SMTAPIGotConnectionErrorHandler();
        public event SMTAPIGotConnectionErrorHandler GotConnectionError;

        protected virtual void onGotConnectionError()
        {
            GotConnectionError?.Invoke();
        }

        public delegate void SMTAPIGotJSONErrorHandler();
        public event SMTAPIGotJSONErrorHandler GotJSONError;

        protected virtual void onGotJSONError()
        {
            GotJSONError?.Invoke();
        }

        /// <summary>
        /// Gets the Channels the "channelname" follows
        /// </summary>
        /// <param name="channelname">channelname</param>
        public async void GetFollowsOfChannel(string channelname)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v3+json");
                string answer = "";

                try
                {
                    string link = "https://api.twitch.tv/kraken/users/" + channelname + "/follows/channels";
                    HttpResponseMessage response = await client.GetAsync(link);
                    response.EnsureSuccessStatusCode();
                    answer = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: {0}", e.ToString());
                    onGotConnectionError();
                    return;
                }

                try
                {
                    SerosTwitchFollowModel model;
                    model = JsonConvert.DeserializeObject<SerosTwitchFollowModel>(answer);
                    onGotFollowChannels(model);
                }
                catch (Newtonsoft.Json.JsonSerializationException e)
                {
                    System.Console.WriteLine("Json-Error: {0}", e.ToString());
                    onGotJSONError();
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the Channels the "channelname" follows
        /// </summary>
        /// <param name="channelname">channelname</param>
        public async void GetFollowsOfChannelNext(SerosTwitchFollowModelLinks nextLink)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v3+json");
                string answer = "";

                try
                {
                    string link = nextLink.Next;
                    HttpResponseMessage response = await client.GetAsync(link);
                    response.EnsureSuccessStatusCode();
                    answer = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: {0}", e.ToString());
                    onGotConnectionError();
                    return;
                }

                try
                {
                    SerosTwitchFollowModel model;
                    model = JsonConvert.DeserializeObject<SerosTwitchFollowModel>(answer);
                    onGotFollowChannels(model);
                }
                catch (Newtonsoft.Json.JsonSerializationException e)
                {
                    System.Console.WriteLine("Json-Error: {0}", e.ToString());
                    onGotJSONError();
                    return;
                }
            }
        }

        /// <summary>
        /// Gets details like if streaming and stream name
        /// </summary>
        /// <param name="channel">The channel</param>
        public async void GetStreamDetails(SerosTwitchFollowModelChannel channel)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v3+json");
                string answer = "";

                try
                {
                    string link = "https://api.twitch.tv/kraken/streams/" + channel.Name;
                    HttpResponseMessage response = await client.GetAsync(link);
                    response.EnsureSuccessStatusCode();
                    answer = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: {0}", e.ToString());
                    onGotConnectionError();
                    return;
                }

                try
                {
                    SerosTwitchFollowModel model;
                    model = JsonConvert.DeserializeObject<SerosTwitchFollowModel>(answer);
                    onGotStreamDetail(channel, model);
                }
                catch (Newtonsoft.Json.JsonSerializationException e)
                {
                    System.Console.WriteLine("Json-Error: {0}", e.ToString());
                    onGotJSONError();
                    return;
                }
            }
        }
    }
}
