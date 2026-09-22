using System;
using System.Numerics;
using System.Text;
using Valve.VR;

namespace ovr;

class Program
{
    static CVRSystem vrSystem = null;

    static StringBuilder sb = new StringBuilder(512);
    static TrackedDevicePose_t[] poseArray = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
    static void Main(string[] args)
    {
        {
            EVRApplicationType appType = EVRApplicationType.VRApplication_Background;
            EVRInitError err = EVRInitError.None;
            
            Console.WriteLine($"Initialising OpenVR as {appType.ToString()}");
            vrSystem = OpenVR.Init(ref err, appType);
            if (err != EVRInitError.None){
                Console.WriteLine($"Could not initialise OpenVR. Error: {err.ToString()}");
                return;
            }
        }
        
        Console.WriteLine("OpenVR runtime version: " +vrSystem.GetRuntimeVersion());
        
        Console.WriteLine("Listing tracked devices.");
        
        
        for (uint i = 0; i < 16; i++)
        {
            bool connected = vrSystem.IsTrackedDeviceConnected(i);
            ETrackedDeviceClass cl = vrSystem.GetTrackedDeviceClass(i);

            ETrackedPropertyError err = ETrackedPropertyError.TrackedProp_Success;
            sb.Clear();
            vrSystem.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_SerialNumber_String,sb, 500, ref err);
            string serial = sb.ToString();
            
            if (err != ETrackedPropertyError.TrackedProp_Success){continue;}

            Console.WriteLine($"Index {i} {connected} - Class {cl.ToString()} - Serial {serial}");
            
        }


        
        vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0.0f, poseArray);
        for (int i = 0; i < poseArray.Length; i++)
        {
            var item = poseArray[i];
            //if (item.bDeviceIsConnected)
            {
                var mat = item.mDeviceToAbsoluteTracking.ToSystemNumericsMatrix();

                Console.Write($"Index {i} Transform: ");

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
                    Console.Write(" ");
                    Console.Write(item.bDeviceIsConnected);
                    Console.WriteLine("");
                }
            }
        }
    }
}
