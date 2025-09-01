using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatedScrolling : MonoBehaviour
{
    public float scrollSpeedY =0.1f;
    private Renderer rend;
    private Material mat;
    private Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
        offset = mat.mainTextureOffset;
    }

    // Update is called once per frame
    void Update()
    {
        offset.y += scrollSpeedY * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}
