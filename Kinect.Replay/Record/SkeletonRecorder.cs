using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;
using System.Reflection;
using System.Linq;

namespace Kinect.Replay.Record
{
	internal class SkeletonRecorder
	{
		private DateTime referenceTime;
		private readonly BinaryWriter writer;

		internal SkeletonRecorder(BinaryWriter writer)
		{
			this.writer = writer;
			referenceTime = DateTime.Now;
		}        

        ColorImagePoint tmpHandRight;

        ColorImagePoint tmpHandLeft;

        ColorImagePoint tmpSpine;

        Skeleton firstSkeleton;
        Skeleton[] totalSkeleton = new Skeleton[6]; 

        //Skeleton[] fixSkleton = new Skeleton[1];
		public void Record(SkeletonFrame frame,KinectSensor psensor)
		{
			writer.Write((int) FrameType.Skeletons);

			var timeSpan = DateTime.Now.Subtract(referenceTime);
			referenceTime = DateTime.Now;
			writer.Write((long) timeSpan.TotalMilliseconds);
			writer.Write((int) frame.TrackingMode);
			writer.Write(frame.FloorClipPlane.Item1);
			writer.Write(frame.FloorClipPlane.Item2);
			writer.Write(frame.FloorClipPlane.Item3);
			writer.Write(frame.FloorClipPlane.Item4);

			writer.Write(frame.FrameNumber);

            //var skeletons = frame.GetSkeletons();

            frame.CopySkeletonDataTo(totalSkeleton);
            firstSkeleton = (from trackskeleton in totalSkeleton 
                             where trackskeleton.TrackingState 
                             == SkeletonTrackingState.Tracked 
                             select trackskeleton).FirstOrDefault();


            if ( firstSkeleton !=null )
            {
                if (firstSkeleton.Joints[JointType.Spine].TrackingState == JointTrackingState.Tracked)
                {
                    tmpHandRight = psensor.CoordinateMapper.
                        MapSkeletonPointToColorPoint(
                        firstSkeleton.Joints[JointType.HandRight].Position,
                        ColorImageFormat.RgbResolution640x480Fps30);

                    tmpHandLeft = psensor.CoordinateMapper.
                        MapSkeletonPointToColorPoint(
                        firstSkeleton.Joints[JointType.HandLeft].Position,
                        ColorImageFormat.RgbResolution640x480Fps30);

                    tmpSpine = psensor.CoordinateMapper.
                        MapSkeletonPointToColorPoint(
                        firstSkeleton.Joints[JointType.Spine].Position,
                        ColorImageFormat.RgbResolution640x480Fps30);

                    writer.Write(tmpHandRight.X);
                    writer.Write(tmpHandRight.Y);
                    writer.Write(tmpHandLeft.X);
                    writer.Write(tmpHandLeft.Y);
                    writer.Write(tmpSpine.X);
                    writer.Write(tmpSpine.Y);
                    writer.Write(true); // is skleton detected
                    //Console.WriteLine("spine x"+tmpSpine.X);
                    //Console.WriteLine("spine y" + tmpSpine.Y);
                    //Console.WriteLine("skleton detected");
                }
                else
                {
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(false); // is skleton detected
                    //Console.WriteLine("skleton NOT DETECTE222");
                }
            }
            else
            {
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(false); // is skleton detected
                //Console.WriteLine("skleton NOT DETECTE");
            }
            

            //frame.CopySkeletonDataTo(skeletons);

            //var formatter = new BinaryFormatter();
            //formatter.Serialize(writer.BaseStream, skeletons);
		}
	}
}