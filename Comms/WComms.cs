using System.IO.MemoryMappedFiles;
using System.Numerics;

namespace OAT_Tracked;

public class WComms : Comms {
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor mmfView;

    public override bool Open(string targetName){
        if (isOpen){return false;}

        int size = (sizeof(float) * 8)+sizeof(int);
        size += sizeof(int) * 2;
        size += sizeof(float) * 3;

        mmf = MemoryMappedFile.CreateOrOpen(targetName, size);
        mmfView = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.ReadWrite);

        isOpen = true;
        return true;
    }

    public override void Write(Vector3 vec)
    {
        if (!isOpen){return;}
        mmfView.Write(0, vec.X);
        mmfView.Write(sizeof(float), vec.Y);
        mmfView.Write(sizeof(float) *2, vec.Z);
    }

    public override void Write(Quaternion quat)
    {
        if (!isOpen){return;}
        mmfView.Write(sizeof(float) *3, quat.W);
        mmfView.Write(sizeof(float) *4, quat.X);
        mmfView.Write(sizeof(float) *5, quat.Y);
        mmfView.Write(sizeof(float) *6, quat.Z);
    }

    public override void Write(float fov)
    {
        if (!isOpen){return;}
        mmfView.Write(sizeof(float) *7, fov);
    }

    public override void Write(int settings)
    {
        if (!isOpen){return;}
        mmfView.Write(sizeof(float)*8, settings);
    }

    public override void Close()
    {
        if (!isOpen){return;}

        isOpen = false;
        mmfView.Dispose();
        mmf.Dispose();
    }
}