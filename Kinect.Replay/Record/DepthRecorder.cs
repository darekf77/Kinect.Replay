using System;
using System.IO;
using Microsoft.Kinect;

namespace Kinect.Replay.Record
{
	internal class DepthRecorder
	{
		private DateTime referenceTime;
		private readonly BinaryWriter writer;
        private DepthImagePixel[] _tmpDepthPixels;
        private DepthImagePoint[] _tmpDepthPoints;
        private KinectSensor _sensor;

		internal DepthRecorder(BinaryWriter writer,KinectSensor sensor)
		{
            _sensor = sensor;
			this.writer = writer;
			referenceTime = DateTime.Now;
            this._tmpDepthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            this._tmpDepthPoints = new DepthImagePoint[sensor.DepthStream.FramePixelDataLength];
		}

		public void Record(DepthImageFrame frame)
		{
			writer.Write((int) FrameType.Depth);

			var timeSpan = DateTime.Now.Subtract(referenceTime);
			referenceTime = DateTime.Now;
			writer.Write((long) timeSpan.TotalMilliseconds);
			writer.Write(frame.BytesPerPixel);
			writer.Write((int) frame.Format);
			writer.Write(frame.Width);
			writer.Write(frame.Height);

			writer.Write(frame.FrameNumber);

			var shorts = new short[frame.PixelDataLength];
            //frame.CopyPixelDataTo(shorts);

            frame.CopyDepthImagePixelDataTo(this._tmpDepthPixels);
            _sensor.CoordinateMapper.MapColorFrameToDepthFrame(
                ColorImageFormat.RgbResolution640x480Fps30,
                DepthImageFormat.Resolution640x480Fps30,
                this._tmpDepthPixels,
                this._tmpDepthPoints
                );

            for (int i = 0; i < shorts.Length; i++)
            {
                shorts[i] = (short)this._tmpDepthPoints[i].Depth;
            }

			writer.Write(shorts.Length);
			foreach (var s in shorts)
				writer.Write(s);
		}
	}
}