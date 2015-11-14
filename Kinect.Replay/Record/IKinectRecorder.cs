using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Replay.Record
{
    interface IKinectRecorder
    {
        Boolean isRecording { get; }
        String FileName { get; }
    }
}
