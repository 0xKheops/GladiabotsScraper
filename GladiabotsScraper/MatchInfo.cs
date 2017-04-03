using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GladiabotsScraper
{
    [DataContract]
    public class MatchInfo
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public Int32 ReplayId { get; set; }

        [DataMember]
        public String Player{get;set;}

        [DataMember]
        public String Opponent { get; set; }

        [DataMember]
        public String MapName { get; set; }

        [DataMember]
        public String Result { get; set; }

        public String DateTimeISO { get { return Date.ToString("u"); } }
    }
}
