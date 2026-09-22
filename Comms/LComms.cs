using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ovr;

public class LComms : Comms
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct nativeDataBlock
    {
        public string name;
        public int length;
        public IntPtr data;
        public int fd;
    };

    [DllImport("comm.so", EntryPoint = "nopen")]
    public static extern int nativeOpen(ref nativeDataBlock dst);

    [DllImport("comm.so", EntryPoint = "nclose")]
    public static extern int nativeClose(ref nativeDataBlock dst);

    nativeDataBlock shm;
    public LComms()
    {
        shm = new nativeDataBlock();
    }

    public override bool Open(string targetName)
    {
        if (isOpen) { return false; }

        if (shm.fd != 0 || shm.data != IntPtr.Zero) { return false; }

        shm.name = "/" + targetName;
        shm.length = (sizeof(float) * 8) + sizeof(int);
        shm.length += sizeof(int) * 2;
        shm.length += sizeof(float) * 3;

        if (nativeOpen(ref shm) == 0)
        {
            isOpen = true;
            return true;
        }
        return false;
    }

    public override void Write(Vector3 vec)
    {
        if (!isOpen) { return; }
        float[] a = { vec.X, vec.Y, vec.Z };
        Marshal.Copy(a, 0, shm.data, 3);
    }

    public override void Write(Quaternion quat)
    {
        if (!isOpen) { return; }
        float[] a = { quat.W, quat.X, quat.Y, quat.Z };
        Marshal.Copy(a, 0, shm.data + sizeof(float) * 3, 4);
    }

    public override void Write(float fov)
    {
        if (!isOpen) { return; }
        float[] a = { fov };
        Marshal.Copy(a, 0, shm.data + sizeof(float) * 7, 1);
    }

    public override void Write(int settings)
    {
        if (!isOpen) { return; }
        Marshal.WriteInt32(shm.data, sizeof(float) * 8, settings);
    }

    public override void Close()
    {
        if (!isOpen) { return; }
        isOpen = false;

        nativeClose(ref shm);
        shm = new nativeDataBlock();

        return;
    }
}