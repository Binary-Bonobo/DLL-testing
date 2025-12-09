using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Globalization;

namespace WinlatorXRBridge
{
    public class WinlatorXRListener : IDisposable
    {
        private UdpClient _udp;
        private Thread _thread;
        private bool _running = false;
        private int _port;
        public XRFrame LastFrame = new XRFrame();

        public WinlatorXRListener(int port = 7278)
        {
            _port = port;
        }

        public void Start()
        {
            if (_running) return;
            _udp = new UdpClient(_port);
            _running = true;
            _thread = new Thread(ThreadProc);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            try { _udp?.Close(); } catch { }
            try { _thread?.Join(500); } catch { }
        }

        void ThreadProc()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, _port);
            while (_running)
            {
                try
                {
                    if (_udp.Available > 0)
                    {
                        var data = _udp.Receive(ref remoteEP);
                        var s = Encoding.ASCII.GetString(data);
                        ParsePacket(s);
                    }
                    else
                    {
                        Thread.Sleep(2);
                    }
                }
                catch (SocketException)
                {
                    // ignore and continue
                }
                catch (Exception ex)
                {
                    Console.WriteLine($""WinlatorXRListener exception: {ex}"");
                }
            }
        }

        public void ParsePacket(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload)) return;
            var tokens = payload.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 27) return;

            // find if last token looks like T/F buttons
            string buttonStr = "";
            int numericCount = tokens.Length;
            if (tokens[tokens.Length - 1].IndexOfAny(new char[] { 'T', 'F', 't', 'f' }) >= 0)
            {
                buttonStr = tokens[tokens.Length - 1];
                numericCount = tokens.Length - 1;
            }

            float[] vals = new float[numericCount];
            try
            {
                for (int i = 0; i < numericCount; i++)
                {
                    vals[i] = float.Parse(tokens[i], CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                return;
            }

            if (vals.Length < 27) return;

            int iidx = 0;

            var f = new XRFrame();

            f.LRot = new Quaternion(vals[iidx++], vals[iidx++], vals[iidx++], vals[iidx++]);
            f.LThumbX = vals[iidx++]; f.LThumbY = vals[iidx++];
            f.LPos = new Vector3(vals[iidx++], vals[iidx++], vals[iidx++]);

            f.RRot = new Quaternion(vals[iidx++], vals[iidx++], vals[iidx++], vals[iidx++]);
            f.RThumbX = vals[iidx++]; f.RThumbY = vals[iidx++];
            f.RPos = new Vector3(vals[iidx++], vals[iidx++], vals[iidx++]);

            f.HRot = new Quaternion(vals[iidx++], vals[iidx++], vals[iidx++], vals[iidx++]);
            f.HPos = new Vector3(vals[iidx++], vals[iidx++], vals[iidx++]);

            if (iidx < vals.Length) f.IPD = vals[iidx++];
            if (iidx < vals.Length) f.FOVX = vals[iidx++];
            if (iidx < vals.Length) f.FOVY = vals[iidx++];
            if (iidx < vals.Length) f.SYNC = vals[iidx++];

            f.ButtonStr = buttonStr.Trim();

            LastFrame = f;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
