using System.Numerics;

namespace WinlatorXRBridge
{
    // Simple container for the frame — keep fields public for quick access
    public class XRFrame
    {
        public Quaternion LRot;
        public float LThumbX;
        public float LThumbY;
        public Vector3 LPos;

        public Quaternion RRot;
        public float RThumbX;
        public float RThumbY;
        public Vector3 RPos;

        public Quaternion HRot;
        public Vector3 HPos;

        public float IPD;
        public float FOVX;
        public float FOVY;
        public float SYNC;

        public string ButtonStr = "";
    }
}
