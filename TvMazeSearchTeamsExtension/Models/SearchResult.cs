using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeSearchTeamsExtension.Models
{
    public class SearchItem
    {
        public double score { get; set; }
        public Show show { get; set; }
    }

    public class Show
    {
        public int id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string language { get; set; }
        public List<string> genres { get; set; }
        public string status { get; set; }
        public int? runtime { get; set; }
        public string premiered { get; set; }
        public string officialSite { get; set; }
        public Image image { get; set; }
        public string summary { get; set; }
    }

    public class Image
    {
        public string medium { get; set; }
        public string original { get; set; }
    }
}
