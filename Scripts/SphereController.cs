using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] GameObject sphere;

    // Initial position of the sphere
    readonly public static float initialX = 0.0f;
    readonly public static float initialY = 8.0f;
    readonly public static float initialZ = 25.0f;

    // Axis radius of ellipse to set position of sphere
    readonly float radiusX = 1.75f;
    readonly float radiusY = 1.5f;

    // Range of depth to set sphere z position
    readonly float depthZ = 10.0f;

    // Adjust the ball speed.
    readonly float speed = 0.2f;

    /// <summary>
    /// Set the sphere in a location visible or hidden from the camera view point
    /// </summary>
    public void SetSphereLocation(bool isRandom)
    {
        if (isRandom)
        {
            sphere.SetActive(true);

            // Set the depth
            float depthFactor = Random.Range(-1.0f, 1.0f);
            float z = initialZ + depthZ * depthFactor;

            // Set x,y within a circle.
            // The radius is variable in proportion to the depth 
            float theta = Random.Range(0.0f, 1.0f) * 2 * (float)System.Math.PI;
            float x = initialX + (radiusX * (depthFactor + 2)) * (float)System.Math.Cos(theta);
            float y = initialY + (radiusY * (depthFactor + 2)) * (float)System.Math.Sin(theta);

            sphere.transform.position = new Vector3(x, y, z);
        }
        else
        {
            sphere.SetActive(false);
            sphere.transform.position = new Vector3(0f, 0f, 0f);
        }
    }

    public void MoveHorizontal(float direction, float speedFactor)
    {
        Vector3 target = sphere.transform.position + new Vector3(direction * 8f, 0f, 0f);
        sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, target, speedFactor * speed);
    }

    public void MoveVertical(float direction, float speedFactor)
    {
        Vector3 target = sphere.transform.position + new Vector3(0f, direction * 8f, 0f);
        sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, target, speedFactor * speed);
    }

    public void MoveDepth(float directionZ, float speedFactor)
    {
        Vector3 target = sphere.transform.position + new Vector3(0f, 0f, - directionZ * 8f);
        sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, target, speedFactor * speed);
    }
}
