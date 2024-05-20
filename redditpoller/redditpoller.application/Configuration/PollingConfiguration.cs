using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redditpoller.application.Configuration
{
    public class PollingConfiguration
    {
        public int Interval { get; set; }

        public int RangeSize { get; set; }
    }
}
