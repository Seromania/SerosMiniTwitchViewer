using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SerosMiniTwitchAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace SerosTwitchViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string saveFileName = "followingChannels.json";
        public static string savedChannelName = "";

        private SerosMiniTwitchAPI.SerosMiniTwitchAPI SmtAPI;

        private ObservableCollection<SerosTwitchFollowModelChannel> followedChannels;

        private System.Timers.Timer getStreamDetailTimer;

        public MainWindow()
        {
            InitializeComponent();

            SmtAPI = new SerosMiniTwitchAPI.SerosMiniTwitchAPI();
            SmtAPI.GotFollowChannels += SmtAPI_GotFollowChannels;
            SmtAPI.GotStreamDetail += SmtAPI_GotStreamDetail;
            SmtAPI.GotConnectionError += SmtAPI_GotConnectionError;
            SmtAPI.GotJSONError += SmtAPI_GotJSONError;

            followedChannels = new ObservableCollection<SerosTwitchFollowModelChannel>();
            lstBox_ChannelListOnline.ItemsSource = followedChannels;

            if (File.Exists(saveFileName))
            {
                loadFollowedChannels();
            }

            getStreamDetailTimer = new System.Timers.Timer();
            getStreamDetailTimer.Elapsed += GetStreamDetailTimer_Elapsed;
            getStreamDetailTimer.Interval = 60000; //1 Minute
            getStreamDetailTimer.Enabled = true;
        }

        private void GetStreamDetailTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetStreamDetailOfAll();
        }

        #region Delegates
        private void SmtAPI_GotStreamDetail(SerosTwitchFollowModelChannel channel, SerosTwitchFollowModel model)
        {
            if (model.Stream == null)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    var chann = followedChannels.Where(item => item.Name == channel.Name).FirstOrDefault();
                    int index = followedChannels.IndexOf(chann);

                    followedChannels[index].Online = false;
                }));
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    var chann = followedChannels.Where(item => item.Name == channel.Name).FirstOrDefault();
                    int index = followedChannels.IndexOf(chann);

                    followedChannels[index].Online = true;
                    followedChannels[index].PreviewPicture = model.Stream.Preview.Medium;
                    followedChannels[index].Viewers = model.Stream.Viewers;
                }));
            }

            //FixMe: Delete later
            //var channm = followedChannels.Where(item => item.Name == channel.Name).FirstOrDefault();
            //Console.WriteLine("Stream {0} is {1}", channm.Name, (channm.Online) ? "online" : "offline");
            //Console.WriteLine("Stream Logo: {0}", channm.Logo);
        }

        private void SmtAPI_GotJSONError()
        {
            MessageBox.Show("Fehler beim Serialisieren!");
        }

        private void SmtAPI_GotConnectionError()
        {
            MessageBox.Show("Fehler bei der Verbindung! Channelname falsch?");
        }

        private void SmtAPI_GotFollowChannels(SerosTwitchFollowModel model)
        {
            Console.WriteLine("Got Follow Channels");
            Console.WriteLine("Following {0} Channels", model.Total);
            Console.WriteLine("Showing {0} of them", model.Follows.Length);

            foreach (SerosTwitchFollowModelFollows follow in model.Follows)
            {
                followedChannels.Add(follow.Channel);
            }

            if ((model.Total > followedChannels.Count) && model.Follows.Length != 0)
            {
                SmtAPI.GetFollowsOfChannelNext(model.Links);
            }
            else
            {
                saveFollowedChannels();
            }
        }

        private void mitem_Refresh_Click(object sender, RoutedEventArgs e)
        {
            followedChannels.Clear();
            SmtAPI.GetFollowsOfChannel(savedChannelName);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeMyChannelNameView cnv = new ChangeMyChannelNameView();
            cnv.ShowDialog();
        }

        private void lstBox_ChannelListOnline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lstbox = sender as ListBox;
            var stream = lstbox.SelectedItem as SerosTwitchFollowModelChannel;

            System.Diagnostics.Process.Start("Livestreamer", "twitch.tv/" + stream.Name + " source");
        }
        #endregion

        #region Load and Save File
        private void saveFollowedChannels()
        {
            Console.WriteLine("===ALL FOLLOWED CHANNELS===");

            JObject jobj = new JObject();
            JArray array = new JArray();
            foreach (SerosTwitchFollowModelChannel channel in followedChannels)
            {
                JObject chanobj = JObject.FromObject(channel);
                array.Add(chanobj);
                Console.WriteLine(channel.Name);
            }
            jobj.Add("channels", array);
            jobj.Add("myChannel", savedChannelName);
            string json = JsonConvert.SerializeObject(jobj, Formatting.None);
            File.WriteAllText(saveFileName, json);
        }

        private void loadFollowedChannels()
        {
            string allread = File.ReadAllText(saveFileName, Encoding.Default);

            SerosTwitchFollowModelSave model;

            try
            {
                model = JsonConvert.DeserializeObject<SerosTwitchFollowModelSave>(allread);
            }
            catch (Newtonsoft.Json.JsonSerializationException e)
            {
                System.Console.WriteLine("Json-Error: {0}", e.ToString());
                return;
            }

            savedChannelName = model.MyChannel;
            Console.WriteLine("===ALL FOLLOWED CHANNELS===");
            foreach (SerosTwitchFollowModelChannel channel in model.Channels)
            {
                Console.WriteLine(channel.Name);
                followedChannels.Add(channel);
            }

            Console.WriteLine("Followed Channels: {0}", followedChannels.Count);

            GetStreamDetailOfAll();
        }

        public class SerosTwitchFollowModelSave
        {
            [JsonProperty("channels")]
            public SerosTwitchFollowModelChannel[] Channels { get; set; }

            [JsonProperty("myChannel")]
            public string MyChannel { get; set; }
        }
        #endregion

        private void GetStreamDetailOfAll()
        {
            foreach (var channel in followedChannels)
            {
                SmtAPI.GetStreamDetails(channel);
            }
        }
    }
}
