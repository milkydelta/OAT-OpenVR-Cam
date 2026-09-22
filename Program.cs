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

    static uint desiredIndex = 0;
    static Matrix4x4 trackerMatrix = Matrix4x4.Identity;
    static Matrix4x4 offsetMatrix = Matrix4x4.Identity;

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

        var ccfg = new ExternalCameraCfg();
        ccfg.LoadFromFile("externalcamera.cfg");
        Console.WriteLine(ccfg.ToString());
        offsetMatrix = ccfg.ToMatrix();

        var com = Comms.New();
        com.Open("uk.lum.vrnyan.cameradata.v1.1");
        com.Write(LIVnyan_cfg.CAM_ON);
        com.Write(ccfg.fov);


        while (true)
        {
            vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0.0f, poseArray);
            if (desiredIndex < poseArray.Length &&
                poseArray[desiredIndex].bDeviceIsConnected &&
                poseArray[desiredIndex].bPoseIsValid)
            {
                var item = poseArray[desiredIndex];
                var mat = item.mDeviceToAbsoluteTracking.ToSystemNumericsMatrix();

                // Convert OpenVR coordinates to Unity.
                // I need to find a way to do this without decomposing the matrix.
                // The below method feels inefficient.
                {
                    Vector3 s, p;
                    Quaternion q;
                    Matrix4x4.Decompose(mat, out s, out q, out p);
                    q.W = -q.W;
                    q.X = -q.X;
                    q.Y = -q.Y;
                    p.Z = -p.Z;
                    mat = Matrix4x4.CreateTranslation(p) * Matrix4x4.CreateFromQuaternion(q);
                }

                trackerMatrix = mat;

                {
                    Vector3 s, p;
                    Quaternion q;
                    Matrix4x4.Decompose(offsetMatrix*trackerMatrix, out s, out q, out p);
                    Console.WriteLine($"{p} {q}");
                    com.Write(p);
                    com.Write(q);

                    // Console.WriteLine(Matrix4x4.CreateFromQuaternion(q));
                    // q.W = -q.W;
                    // Console.WriteLine(Matrix4x4.CreateFromQuaternion(q));
                }

            }


            //return;
            System.Threading.Thread.Sleep(1000/90);

        }


        
        vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0.0f, poseArray);
        for (int i = 0; i < poseArray.Length; i++)
        {
            var item = poseArray[i];
            if (item.bDeviceIsConnected)
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
                    Console.Write(" ");
                    Console.Write(item.bPoseIsValid);
                    Console.WriteLine("");
                }
            }
        }
    }
}
