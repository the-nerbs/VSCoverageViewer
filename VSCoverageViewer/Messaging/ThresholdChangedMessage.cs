using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Messaging
{
    /// <summary>
    /// Message type sent when the coverage threshold changes.
    /// </summary>
    internal class ThresholdChangedMessage
    {
        /// <summary>
        /// Gets the new coverage threshold percentage.
        /// </summary>
        public double NewThresholdRatio { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="ThresholdChangedMessage"/>.
        /// </summary>
        /// <param name="newRatio">The new coverage threshold.</param>
        public ThresholdChangedMessage(double newRatio)
        {
            NewThresholdRatio = newRatio;
        }
    }
}
