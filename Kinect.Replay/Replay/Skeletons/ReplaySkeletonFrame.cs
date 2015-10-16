using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;

namespace Kinect.Replay.Replay.Skeletons
{
	public class ReplaySkeletonFrame : ReplayFrame
	{
		public Tuple<float, float, float, float> FloorClipPlane { get; private set; }
        //public Skeleton[] Skeletons { get; private set; }
		public SkeletonTrackingMode TrackingMode { get; set; }

        public int SpinePositionX;
        public int SpinePositionY;
        public int LeftHandPositionX;
        public int LeftHandPositionY;
        public int RightHandPositionX;
        public int RightHandPositionY;
        public bool IsSkeletonDetected;

		public ReplaySkeletonFrame(SkeletonFrame frame)
		{
			FloorClipPlane = frame.FloorClipPlane;
			FrameNumber = frame.FrameNumber;
			TimeStamp = frame.Timestamp;
            //Skeletons = frame.GetSkeletons();
			TrackingMode = frame.TrackingMode;
		}

		public ReplaySkeletonFrame()
		{
		}

		internal override void CreateFromReader(BinaryReader reader)
		{
			TimeStamp = reader.ReadInt64();
			TrackingMode = (SkeletonTrackingMode)reader.ReadInt32();
			FloorClipPlane = new Tuple<float, float, float, float>(
				reader.ReadSingle(), reader.ReadSingle(),
				reader.ReadSingle(), reader.ReadSingle());

			FrameNumber = reader.ReadInt32();
            RightHandPositionX = reader.ReadInt32();
            RightHandPositionY = reader.ReadInt32();
            LeftHandPositionX = reader.ReadInt32();
            LeftHandPositionY = reader.ReadInt32();
            SpinePositionX = reader.ReadInt32();
            SpinePositionY = reader.ReadInt32();
            IsSkeletonDetected = reader.ReadBoolean();

            //var formatter = new BinaryFormatter();
            //Skeletons = (Skeleton[])formatter.Deserialize(reader.BaseStream);
		}

		public static implicit operator ReplaySkeletonFrame(SkeletonFrame frame)
		{
			return new ReplaySkeletonFrame(frame);
		}
	}
}