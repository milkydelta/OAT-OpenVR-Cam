using System;
using System.Runtime.InteropServices;

namespace OAT_Tracked;

[Flags]
public enum LIVnyan_cfg : int {
    None         = 0b0000_0000,
    CAM_ON       = 0b0000_0001,
    LOG_ON       = 0b0000_0010,
    LOGSPM       = 0b0000_0100,
    OAT_READCLIP = 0b0000_1000
}

public abstract class Comms{
    internal bool isOpen=false;
    internal LIVnyan_cfg set = LIVnyan_cfg.LOG_ON;

    public static Comms New()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
            return new LComms();
        }
        return new WComms();
    }
    abstract public bool Open(string targetName);
    abstract public void Write(System.Numerics.Vector3 vec);
    abstract public void Write(System.Numerics.Quaternion quat);
    abstract public void Write(float fov);
    abstract public void Write(int settings);
    abstract public void Close();

    public void Write(LIVnyan_cfg settings)
    {
        set = settings;
        Write((int)settings);
    }

    public bool Toggle(LIVnyan_cfg settings)
    {
        if (set.HasFlag(settings))
        {
            Write(set & ~settings);
            Console.WriteLine("OFF");
            return false;
        }
        else
        {
            Write(set | settings);
            Console.WriteLine("ON");
            return true;
        }
    }
}
