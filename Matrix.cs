using System.Numerics;

namespace OAT_Tracked;

public static class Extensions
{
    public static System.Numerics.Matrix4x4 ToSystemNumericsMatrix(this Valve.VR.HmdMatrix34_t hmdM) {
        Matrix4x4 mat = Matrix4x4.Identity;

		mat.M11 = hmdM.m0;
		mat.M12 = hmdM.m1;
		mat.M13 = hmdM.m2;
		mat.M14 = hmdM.m3;

		mat.M21 = hmdM.m4;
		mat.M22 = hmdM.m5;
		mat.M23 = hmdM.m6;
		mat.M24 = hmdM.m7;

		mat.M31 = hmdM.m8;
		mat.M32 = hmdM.m9;
		mat.M33 = hmdM.m10;
		mat.M34 = hmdM.m11;

        return mat;
    }
}