using System.Numerics;

namespace WinlatorXRBridge
{
    public class XRFrame
    {
        public Quaternion LRot = Quaternion.Identity;
        public Vector3 LPos = Vector3.Zero;
        public float LThumbX = 0f;
        public float LThumbY = 0f;

        public Quaternion RRot = Quaternion.Identity;
        public Vector3 RPos = Vector3.Zero;
        public float RThumbX = 0f;
        public float RThumbY = 0f;

        public Quaternion HRot = Quaternion.Identity;
        public Vector3 HPos = Vector3.Zero;
        public float IPD = 0.064f;
        public float FOVX = 0f;
        public float FOVY = 0f;
        public float SYNC = 0f;

        public string ButtonStr = "";
    }
}
