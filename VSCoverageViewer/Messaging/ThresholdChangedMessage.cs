using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Messaging
{
    class ThresholdChangedMessage
    {
        public double NewThresholdRatio { get; private set; }


        public ThresholdChangedMessage(double newRatio)
        {
            NewThresholdRatio = newRatio;
        }
    }
}
