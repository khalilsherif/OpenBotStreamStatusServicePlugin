using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBot.Plugins;
using System.IO.Pipes;
using System.Threading;

namespace OpenBotStreamStatusServicePlugin.Services
{
    class StreamStatusService : AbstractService
    {
        public Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public bool Streaming
        {
            get
            {
                return _isStreaming;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _streamStartTime;
            }
        }

        public DateTime StopTime
        {
            get
            {
                return _streamStopTime;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (_isStreaming)
                    return DateTime.Now - _streamStartTime;
                else
                    return _streamStopTime - _streamStartTime;
            }
        }

        public override string Description
        {
            get
            {
                return "Provides information about stream start, end, and duration times";
            }
        }

        public override bool HasPreferences
        {
            get
            {
                return false;
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public delegate void OnStreamStatusChanged(bool status);

        private bool _isStreaming;
        private string _pipeName = "OBSStreamStatusPlugin_OpenBot";
        private DateTime _streamStartTime;
        private DateTime _streamStopTime;
        private event OnStreamStatusChanged _streamStatusChanged;

        public event OnStreamStatusChanged StreamStatusChanged
        {
            add
            {
                lock (this)
                {
                    _streamStatusChanged += value;
                }
            }

            remove
            {
                lock (this)
                {
                    _streamStatusChanged -= value;
                }
            }
        }

        public StreamStatusService()
        {
            _streamStartTime = DateTime.MinValue;
            _streamStopTime = DateTime.MinValue;

            _isStreaming = false;
        }

        private void AwaitNamePipeData()
        {
            try
            {
                NamedPipeServerStream _server = new NamedPipeServerStream(_pipeName, PipeDirection.In);

                while (true)
                {
                    _server.WaitForConnection();

                    byte[] buffer = new byte[1];
                    int nRead = _server.Read(buffer, 0, 1);

                    if (nRead > 0)
                    {
                        bool newValue = buffer[0] == 1;
                        if (newValue)
                        {
                            _streamStartTime = DateTime.Now;
                        }
                        else
                        {
                            _streamStopTime = DateTime.Now;
                        }

                        if (newValue != _isStreaming)
                            if (_streamStatusChanged != null)
                                _streamStatusChanged.Invoke(newValue);

                        _isStreaming = newValue;
                    }

                    _server.Disconnect();
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        Thread T;
        public override bool LoadService()
        {
            T = new Thread(AwaitNamePipeData);
            T.IsBackground = true;
            T.Start();

            return true;
        }

        public override void ShowPreferences()
        {
            
        }

        public override void UnloadService()
        {
            T.Abort();
        }
    }
}
