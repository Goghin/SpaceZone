using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
      MeshRenderer mr = GetComponent<MeshRenderer>();

      Material mat = mr.material;

      Vector2 offset = mat.mainTextureOffset;

      offset.y += Time.deltaTime / 70f; 

      mat.mainTextureOffset = offset;
    }
}
