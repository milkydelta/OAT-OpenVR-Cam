using System;
using System.Numerics;
using System.Text;
using Valve.VR;

namespace ovr;

class Program
{
    static Valve.VR.CVRSystem vrSystem = null;

    static StringBuilder sb = new StringBuilder(512);
    static TrackedDevicePose_t[] poseArray = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
    static void Main(string[] args)
    {
        Valve.VR.EVRInitError initError = Valve.VR.EVRInitError.None;

        Console.WriteLine("Hello, World!");
        vrSystem = Valve.VR.OpenVR.Init(ref initError, Valve.VR.EVRApplicationType.VRApplication_Background);
        if (initError != Valve.VR.EVRInitError.None){ return;}
        Console.WriteLine("OpenVR runtime version: " +vrSystem.GetRuntimeVersion());
        
        for (uint i = 0; i < 16; i++)
        {
            if (vrSystem.IsTrackedDeviceConnected(i))
            {
                Console.Write($"Index {i}: ");
                Console.Write(vrSystem.GetTrackedDeviceClass(i));
                Console.Write(" ");
                Valve.VR.ETrackedPropertyError err = 0;
                vrSystem.GetStringTrackedDeviceProperty(i, Valve.VR.ETrackedDeviceProperty.Prop_SerialNumber_String,sb, 500, ref err);
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }
        }


        
        vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0.0f, poseArray);
        for (int i = 0; i < poseArray.Length; i++)
        {
            var item = poseArray[i];
            if (item.bDeviceIsConnected)
            {
                var mat = item.mDeviceToAbsoluteTracking.ToSystemNumericsMatrix();

                Console.Write($"Index {i} Pos: ");
                Console.WriteLine(mat.Translation);

                Vector3 pos;
                Quaternion rot;
                Vector3 scale;
                if (Matrix4x4.Decompose(mat, out scale, out rot, out pos))
                {
                    Console.Write(pos);
                    Console.Write(" ");
                    Console.Write(rot);
                    Console.Write(" ");
                    Console.Write(scale);
                    Console.WriteLine(" ");
                }
            }
        }
    }
}
