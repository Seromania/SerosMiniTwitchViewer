using System.ComponentModel;
using Newtonsoft.Json;

namespace SerosMiniTwitchAPI
{
    public class SerosTwitchFollowModel
    {
        [JsonProperty("_links")]
        public SerosTwitchFollowModelLinks Links { get; set; }

        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("follows")]
        public SerosTwitchFollowModelFollows[] Follows { get; set; }

        [JsonProperty("stream")]
        public SerosTwitchFollowModelFollows Stream { get; set; }
    }

    public class SerosTwitchFollowModelLinks
    {
        //public IList<string> _links { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("self")]
        public string Self { get; set; }


    }

    public class SerosTwitchFollowModelFollows
    {
        [JsonProperty("channel")]
        public SerosTwitchFollowModelChannel Channel { get; set; }

        [JsonProperty("preview")]
        public SerosTwitchFollowModelPreview Preview { get; set; }

        [JsonProperty("viewers")]
        public int Viewers { get; set; }
    }

    public class SerosTwitchFollowModelPreview
    {
        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("Large")]
        public string Large { get; set; }
    }

    public class SerosTwitchFollowModelChannel : System.ComponentModel.INotifyPropertyChanged
    {
        
        private string displayName;
        [JsonProperty("display_name")]
        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }
 
        private string name;
        [JsonProperty("name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        
        private string status;
        [JsonProperty("status")]
        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private string logo;
        [JsonProperty("logo")]
        public string Logo
        {
            get
            {
                return logo;
            }

            set
            {
                logo = value;
                OnPropertyChanged("Logo");
            }
        }

        private bool online;
        public bool Online
        {
            get
            {
                return online;
            }

            set
            {
                online = value;
                OnPropertyChanged("Online");
            }
        }

        private string previewPicture;
        public string PreviewPicture
        {
            get
            {
                return previewPicture;
            }

            set
            {
                previewPicture = value;
                OnPropertyChanged("PreviewPicture");
            }
        }

        private int viewers;
        public int Viewers
        {
            get
            {
                return viewers;
            }

            set
            {
                viewers = value;
                OnPropertyChanged("Viewers");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
