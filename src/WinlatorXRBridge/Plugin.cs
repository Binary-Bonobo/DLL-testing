using BepInEx;
using UnityEngine;
using System;
using Quaternion = UnityEngine.Quaternion;
using Vector3u = UnityEngine.Vector3;

namespace WinlatorXRBridge
{
    [BepInPlugin("com.yourname.winlatorxrbridge", "WinlatorXR Bridge", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private WinlatorXRListener _listener;
        private XRFrame _lastFrame;
        private float _smoothing = 0.9f;
        private GameObject _leftHandObj;
        private GameObject _rightHandObj;

        void Awake()
        {
            Logger.LogInfo("WinlatorXR Bridge loading...");
            _listener = new WinlatorXRListener(7278);
            try
            {
                _listener.Start();
                Logger.LogInfo("Started WinlatorXR UDP listener on port 7278.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to start listener: {ex}");
            }

            _leftHandObj = FindHandObject(false);
            _rightHandObj = FindHandObject(true);
        }

        private GameObject FindHandObject(bool right)
        {
            string[] namesRight = new[] { "RightHand", "Right Hand", "HandRight", "Hand_R", "controller_right" };
            string[] namesLeft  = new[] { "LeftHand", "Left Hand", "HandLeft", "Hand_L", "controller_left" };

            var arr = right ? namesRight : namesLeft;
            foreach (var n in arr)
            {
                var go = GameObject.Find(n);
                if (go != null) return go;
            }

            var all = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (var go in all)
            {
                if (go.name.IndexOf("hand", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    ((right && go.name.IndexOf("right", StringComparison.OrdinalIgnoreCase) >= 0) ||
                     (!right && go.name.IndexOf("left", StringComparison.OrdinalIgnoreCase) >= 0)))
                    return go;
            }
            return null;
        }

        void OnDestroy()
        {
            _listener?.Stop();
            _listener = null;
        }

        void Update()
        {
            if (_listener == null) return;
            var frame = _listener.LastFrame;
            if (frame == null) return;

            var headPos = new Vector3u(frame.HPos.X, frame.HPos.Y, frame.HPos.Z);
            var headRot = new Quaternion((float)frame.HRot.X, (float)frame.HRot.Y, (float)frame.HRot.Z, (float)frame.HRot.W);

            if (Camera.main != null)
            {
                Camera.main.transform.position = Vector3u.Lerp(Camera.main.transform.position, headPos, 1f - _smoothing);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, headRot, 1f - _smoothing);
            }

            if (_leftHandObj != null)
            {
                var lpos = new Vector3u(frame.LPos.X, frame.LPos.Y, frame.LPos.Z);
                var lrot = new Quaternion((float)frame.LRot.X, (float)frame.LRot.Y, (float)frame.LRot.Z, (float)frame.LRot.W);
                _leftHandObj.transform.position = Vector3u.Lerp(_leftHandObj.transform.position, lpos, 1f - _smoothing);
                _leftHandObj.transform.rotation = Quaternion.Slerp(_leftHandObj.transform.rotation, lrot, 1f - _smoothing);
            }

            if (_rightHandObj != null)
            {
                var rpos = new Vector3u(frame.RPos.X, frame.RPos.Y, frame.RPos.Z);
                var rrot = new Quaternion((float)frame.RRot.X, (float)frame.RRot.Y, (float)frame.RRot.Z, (float)frame.RRot.W);
                _rightHandObj.transform.position = Vector3u.Lerp(_rightHandObj.transform.position, rpos, 1f - _smoothing);
                _rightHandObj.transform.rotation = Quaternion.Slerp(_rightHandObj.transform.rotation, rrot, 1f - _smoothing);
            }
        }
    }
}
