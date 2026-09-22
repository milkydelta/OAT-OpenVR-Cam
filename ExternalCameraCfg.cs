using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;



namespace ovr;

class ExternalCameraCfg {
    public float x,y,z = 0f;
    public float rx,ry,rz = 0f;
    public float fov = 0f;

    private void Reset()
    {
        x = 0f;
        y = 0f;
        z = 0f;
        rx = 0f;
        ry = 0f;
        rz = 0f;
        fov = 0f;
    }

    public void LoadFromFile(string path)
    {
        if (!File.Exists(path)) {
            Console.WriteLine("LoadFromFile: file does not exist.");
            return;
        }

        Dictionary<string,string> keyvalues = new Dictionary<string, string>();

        foreach (string line in File.ReadAllLines(path)) {
            if (line == ""){continue;}
            var t = line.Split(new[] {'='}, 2);
            if (t.Length == 2) {keyvalues[t[0]] = t[1];}
        }

        LoadFromDict(keyvalues);
    }

    public void LoadFromDict(Dictionary<string, string> dict) {
        Reset();

        if (dict.TryGetValue("x", out string xs)) {
            Single.TryParse(xs,out x);
        }
        if (dict.TryGetValue("y", out string ys)) {
            Single.TryParse(ys,out y);
        }
        if (dict.TryGetValue("z", out string zs)) {
            Single.TryParse(zs,out z);
        }

        if (dict.TryGetValue("rx", out string rxs)) {
            Single.TryParse(rxs,out rx);
        }
        if (dict.TryGetValue("ry", out string rys)) {
            Single.TryParse(rys,out ry);
        }
        if (dict.TryGetValue("rz", out string rzs)) {
            Single.TryParse(rzs,out rz);
        }

        if (dict.TryGetValue("fov", out string fovs)) {
            Single.TryParse(fovs,out fov);
        }
    }

    static float DtoR(float degrees)
    {
        return (float)(degrees * Math.PI / 180d);
    }

    public (Vector3,Quaternion) ToVecQuat()
    {
        var vec = new Vector3(x, y, z);
        var quat = Quaternion.CreateFromYawPitchRoll(DtoR(ry), DtoR(rx), DtoR(rz));
        return (vec, quat);
    }
    public Matrix4x4 ToMatrix()
    {
        var t = Matrix4x4.CreateTranslation(x,y,z);
        var r = Matrix4x4.CreateFromYawPitchRoll(DtoR(ry), DtoR(rx), DtoR(rz));
        return t * r;
    }
    public override string ToString()
    {
        return String.Concat("x=",x," y=",y," z=",z," rx=",rx," ry=",ry," rz=",rz," fov=",fov);
    }
}