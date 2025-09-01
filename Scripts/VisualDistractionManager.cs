using System.Collections;
using UnityEngine;

public class VisualDistractionManager : MonoBehaviour
{
    [Header("Spawn Area Settings")]
    public Transform spawnCenter; // Center of spawn region
    public Vector3 spawnBounds = new Vector3(3f, 2f, 3f); // Width, Height, Depth

    [Header("Random Movement (3D)")]
    public GameObject[] movingDistractions;
    public float moveSpeed = 1f;

    [Header("Pop-In for 3 Secs (3D)")]
    public GameObject[] timedDistractions;

    [Header("Fade In/Out (3D with Renderer)")]
    public GameObject[] fadingDistractions;
    public float fadeDuration = 2f;
    public float visibleDuration = 3f;

    public void StartVisuals()
    {
        StartCoroutine(HandleMovingDistractions());
        StartCoroutine(HandleTimedDistractions());
        StartCoroutine(HandleFadingDistractions());
    }

    public void StopVisuals()
    {
        StopAllCoroutines();

        foreach (var obj in movingDistractions) obj.SetActive(false);
        foreach (var obj in timedDistractions) obj.SetActive(false);
        foreach (var obj in fadingDistractions) obj.SetActive(false);
    }

    // -------------------------------
    // Random Movement
    // -------------------------------
    IEnumerator HandleMovingDistractions()
    {
        foreach (var obj in movingDistractions)
        {
            obj.SetActive(true);
            StartCoroutine(MoveRandomly(obj));
        }
        yield return null;
    }

    IEnumerator MoveRandomly(GameObject obj)
    {
        Transform tf = obj.transform;
        while (true)
        {
            Vector3 target = RandomPositionInBounds();
            Vector3 start = tf.position;
            float t = 0f;

            while (t < 1f)
            {
                tf.position = Vector3.Lerp(start, target, t);
                t += Time.deltaTime * moveSpeed;
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // -------------------------------
    // Timed Pop-In (Capsule)
    // -------------------------------
    IEnumerator HandleTimedDistractions()
    {
        while (true)
        {
            foreach (var obj in timedDistractions)
                obj.SetActive(false);

            GameObject chosen = timedDistractions[Random.Range(0, timedDistractions.Length)];
            chosen.transform.position = RandomPositionInBounds();
            chosen.SetActive(true);

            yield return new WaitForSeconds(3f); // stay visible

            chosen.SetActive(false); // pop out
            yield return new WaitForSeconds(1f);
        }
    }

    // -------------------------------
    // Fading From Sides (Quad)
    // -------------------------------
    IEnumerator HandleFadingDistractions()
    {
        while (true)
        {
            GameObject chosen = fadingDistractions[Random.Range(0, fadingDistractions.Length)];
            Renderer rend = chosen.GetComponent<Renderer>();
            if (rend == null) yield break;

            bool fromLeft = Random.value > 0.5f;
            Vector3 start = RandomSidePosition(fromLeft);
            Vector3 end = spawnCenter.position + new Vector3(0, start.y - spawnCenter.position.y, start.z - spawnCenter.position.z);

            chosen.transform.position = start;
            chosen.SetActive(true);

            Color originalColor = rend.material.color;
            originalColor.a = 0f;
            rend.material.color = originalColor;

            // Fade in and drift
            for (float t = 0f; t < 1f; t += Time.deltaTime / fadeDuration)
            {
                float eased = Mathf.SmoothStep(0f, 1f, t);
                rend.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, eased);
                chosen.transform.position = Vector3.Lerp(start, end, eased);
                yield return null;
            }

            yield return new WaitForSeconds(visibleDuration);

            // Fade out and finish drift
            for (float t = 1f; t > 0f; t -= Time.deltaTime / fadeDuration)
            {
                float eased = Mathf.SmoothStep(0f, 1f, t);
                rend.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, eased);
                chosen.transform.position = Vector3.Lerp(start, end, eased);
                yield return null;
            }

            chosen.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }

    // -------------------------------
    // Helpers
    // -------------------------------
    Vector3 RandomPositionInBounds()
    {
        Vector3 offset = new Vector3(
            Random.Range(-spawnBounds.x / 2f, spawnBounds.x / 2f),
            Random.Range(-spawnBounds.y / 2f, spawnBounds.y / 2f),
            Random.Range(-spawnBounds.z / 2f, spawnBounds.z / 2f)
        );
        return spawnCenter.position + offset;
    }

    Vector3 RandomSidePosition(bool fromLeft)
    {
        float x = fromLeft ? -spawnBounds.x / 2f : spawnBounds.x / 2f;
        float y = Random.Range(-spawnBounds.y / 2f, spawnBounds.y / 2f);
        float z = Random.Range(-spawnBounds.z / 2f, spawnBounds.z / 2f);
        return spawnCenter.position + new Vector3(x, y, z);
    }
}
