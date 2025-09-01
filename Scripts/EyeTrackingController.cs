using UnityEngine;
using Unity.XR.PXR;

public class EyeTrackingController : MonoBehaviour
{

    /// <summary>
    /// Provides the eyes origin and orientation vector.
    /// This method uses Pico XR SDK to read the eye direction, head position and head orientation
    /// Throws an exception if the XR module fails to read eye information
    /// </summary>
    public static (Vector3 origin, Vector3 vector) GetGazeData()
    {
        Vector3 vector;
        Matrix4x4 matrix;

        bool eyeReadResult = PXR_EyeTracking.GetCombineEyeGazeVector(out vector);
        bool headReadResult = PXR_EyeTracking.GetHeadPosMatrix(out matrix);

        if (!eyeReadResult)
        {
            throw new System.Exception("Eye reading failed");
        }

        if (!headReadResult)
        {
            throw new System.Exception("Head reading failed");
        }

        // Get the translation
        Vector4 t_ = matrix.GetColumn(3);
        Vector3 origin = new(t_.x, t_.y, t_.z);

        // Get the rotation quaternion
        Quaternion rotation = QuaternionFromMatrix(matrix);

        // Rotate gaze vector
        vector = rotation * vector;

        return (origin, vector);
    }
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }

}
