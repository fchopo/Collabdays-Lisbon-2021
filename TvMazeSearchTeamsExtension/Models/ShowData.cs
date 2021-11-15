using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeSearchTeamsExtension.Models
{
    public class ShowData
    {
        public string id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string language { get; set; }
        public string status { get; set; }
        public string officialSite { get; set; }

        public ShowImage image { get; set; }

        public string summary { get; set; }

    }

    public class ShowImage
    {
        public string medium { get; set; }

        public string original { get; set; }
    }
}
