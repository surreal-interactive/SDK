using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SVRCommon
{
    public static Pose PoseMultiply(Pose lhs, Pose rhs)
    {
        var ret = new Pose();
        ret.position = lhs.position + lhs.rotation * rhs.position;
        ret.rotation = lhs.rotation * rhs.rotation;
        return ret;
    }

    public static Pose PoseInverse(Pose pose)
    {
        Pose ret;
        ret.rotation = Quaternion.Inverse(pose.rotation);
        ret.position = ret.rotation * -pose.position;
        return ret;
    }

    public static Vector3 QuatMulVec(Quaternion rotation, Vector3 point)
    {
        float num = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num8 = rotation.x * num3;
        float num9 = rotation.y * num3;
        float num10 = rotation.w * num;
        float num11 = rotation.w * num2;
        float num12 = rotation.w * num3;
        Vector3 result = default(Vector3);
        result.x =
            (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
        result.y =
            (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
        result.z =
            (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
        return result;
    }
}
