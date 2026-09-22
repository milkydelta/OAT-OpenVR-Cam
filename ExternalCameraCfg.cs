using System;
using System.Collections.Generic;


namespace ovr;

class ExternalCameraCfg {
    public float x,y,z = 0f;
    public float rx,ry,rz = 0f;
    public float fov = 0f;
    public int index = 0;

    public ExternalCameraCfg(Dictionary<string, string> dict) {
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

        if (dict.TryGetValue("index", out string indexs)) {
            Int32.TryParse(indexs,out index);
            if (index >15) {index=0;}
        }
    }
    public override string ToString()
    {
        return String.Concat("x=",x," y=",y," z=",z," rx=",rx," ry=",ry," rz=",rz," fov=",fov);
    }
}