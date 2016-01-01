using System;
using System.IO;
using System.Linq;
using Microsoft.Kinect;

namespace Kinect.Replay.Record
{
    public class KinectRecorder : IKinectRecorder
	{
		private Stream recordStream;
		private string recordFileName;
		private readonly KinectSensor _sensor;
		private readonly BinaryWriter writer;

		private DateTime previousFlushDate;

		private readonly ColorRecorder colorRecoder;
		private readonly DepthRecorder depthRecorder;
		private readonly SkeletonRecorder skeletonRecorder;
		private readonly AudioRecorder audioRecorder;

		public KinectRecordOptions Options { get; set; }
        private Boolean _IsRecording = false;

		public KinectRecorder(KinectRecordOptions options, string targetFileName, KinectSensor sensor)
		{
            Console.WriteLine("START!!");
			Options = options;
			recordFileName = targetFileName;
			_sensor = sensor;
			var stream = File.Create(targetFileName);
            Console.WriteLine("Stream created");
			recordStream = stream;
			writer = new BinaryWriter(recordStream);

			writer.Write((int)Options);
            Console.WriteLine("before arr");
            //var colorToDepthRelationalParameters = sensor.CoordinateMapper.ColorToDepthRelationalParameters.ToArray();
            //writer.Write(colorToDepthRelationalParameters.Length);
            //writer.Write(colorToDepthRelationalParameters);
            Console.WriteLine("shit after");

			if ((Options & KinectRecordOptions.Frames) != 0)
			{
				colorRecoder = new ColorRecorder(writer);
				depthRecorder = new DepthRecorder(writer,sensor);
				skeletonRecorder = new SkeletonRecorder(writer);
			}

			if ((Options & KinectRecordOptions.Audio) != 0)
				audioRecorder = new AudioRecorder();

			previousFlushDate = DateTime.Now;
            Console.WriteLine("recording!");
            _IsRecording = true;
		}

		public void Record(SkeletonFrame frame,KinectSensor psensor)
		{
			if (skeletonRecorder == null)
				return;
			if (writer == null)
				throw new Exception("This recorder is stopped");

            skeletonRecorder.Record(frame, psensor);
			Flush();
		}

		public void Record(ColorImageFrame frame)
		{
			if (colorRecoder == null)
				return;
			if (writer == null)
				throw new Exception("This recorder is stopped");

			colorRecoder.Record(frame);
			Flush();
		}

		public void Record(DepthImageFrame frame)
		{
			if (depthRecorder == null)
				return;
			if (writer == null)
				throw new Exception("This recorder is stopped");

			depthRecorder.Record(frame);
			Flush();
		}

		public void StartAudioRecording()
		{
			if (audioRecorder == null || audioRecorder.IsRunning)
				return;
			audioRecorder.RecordDefaultDeviceAudio();
		}

		private void Flush()
		{
			var now = DateTime.Now;

			if (now.Subtract(previousFlushDate).TotalSeconds > 60)
			{
				previousFlushDate = now;
				writer.Flush();
			}
		}

		public void Stop()
		{
            _IsRecording = false;
			if (writer == null)
				throw new Exception("This recorder is already stopped");

			if (audioRecorder != null && audioRecorder.IsRunning)
				audioRecorder.StopDefaultAudioRecording(Path.ChangeExtension(recordFileName, ".wav"));

			writer.Close();
			writer.Dispose();


			recordStream.Dispose();
			recordStream = null;
            
		}


        public bool isRecording
        {
            get { return _IsRecording; }
        }


        public String FileName
        {
            get { return recordFileName; }
        }
    }
}