using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Kinect.Replay.Record;
using Kinect.Replay.Replay.Skeletons;
using Microsoft.Kinect;

namespace Kinect.Replay.Replay
{
	public interface IKinectReplay
	{
        int DepthDataPixelLength { get; }
        int ColorDataPixelLength { get; }
	}
}