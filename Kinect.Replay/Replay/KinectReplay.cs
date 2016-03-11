using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Kinect.Replay.Record;
using Kinect.Replay.Replay.Skeletons;
using Microsoft.Kinect;

namespace Kinect.Replay.Replay
{
	public class KinectReplay : IKinectReplay, IDisposable
	{
		private BinaryReader reader;
		private Stream stream;
		private readonly SynchronizationContext synchronizationContext;

		public event Action<ReplayAllFramesReadyEventArgs> AllFramesReady;
		public event Action ReplayFinished;

        //public CoordinateMapper CoordinateMapper { get; private set; }

		private ReplayAllFramesSystem framesReplay;
		public KinectRecordOptions Options { get; private set; }

		public bool Started { get; internal set; }
		public string AudioFilePath { get; set; }

		public bool IsFinished { get { return framesReplay == null || framesReplay.IsFinished; } }

        public void setPauseMode(Boolean mode) {
            framesReplay.IsPause = mode;
        }

        public void nextFrame()
        {
            framesReplay.nextFrame();
        }


		public KinectReplay(string fileName)
		{
            Started = false;
			stream = File.OpenRead(fileName);
			reader = new BinaryReader(stream);

			synchronizationContext = SynchronizationContext.Current;

			Options = (KinectRecordOptions)reader.ReadInt32();
            //var paramsArrayLength = reader.ReadInt32();
            //var colorToDepthRelationalParameters = reader.ReadBytes(paramsArrayLength);
            //CoordinateMapper = new CoordinateMapper(colorToDepthRelationalParameters);

			if ((Options & KinectRecordOptions.Frames) != 0)
			{
				framesReplay = new ReplayAllFramesSystem();
				framesReplay.AddFrames(reader);
				framesReplay.ReplayFinished += () => synchronizationContext.Send(state => ReplayFinished.Raise(),null);
			}
			if ((Options & KinectRecordOptions.Audio) != 0)
			{
				var audioFilePath = Path.ChangeExtension(fileName, ".wav");
				if (File.Exists(audioFilePath))
					AudioFilePath = audioFilePath;
			}
            
		}

		public void Start()
		{
            if (Started)
            {
                Console.WriteLine("KinectReplay already started");

                return;
            }               

			Started = true;

			if (framesReplay == null) return;

			framesReplay.Start();
			framesReplay.FrameReady += frame => synchronizationContext
					 .Send(state => AllFramesReady.Raise(new ReplayAllFramesReadyEventArgs { AllFrames = frame }), null);
		}

		public void Stop()
		{
            
			if (framesReplay != null)
				framesReplay.Stop();
            Started = false;
			if(framesReplay!=null)
            framesReplay.FrameReady -= frame => synchronizationContext
                     .Send(state => AllFramesReady.Raise(new ReplayAllFramesReadyEventArgs { AllFrames = frame }), null);
            
		}

		public void Dispose()
		{
			Stop();

			framesReplay = null;

			if (reader != null)
			{
				reader.Dispose();
				reader = null;
			}

			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}
		}

        //TODO make this dynamic
        public int DepthDataPixelLength
        {
            get { return 307200; }
        }

        public int ColorDataPixelLength
        {
            get { return 1228800; }
        }


        public int MaxDepth
        {
            get { return 4000; }
        }

        public int MinDepth
        {
            get { return 800; }
        }
    }
}