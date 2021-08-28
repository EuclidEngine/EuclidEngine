using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEAreaPlane : MonoBehaviour
{
    new public Camera camera;
    public Rect rect;
    public Vector2 size;

    private Texture2D texture2D;
    new private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start() 
    {
        texture2D = new Texture2D(Screen.width, Screen.height);
        renderer = GetComponent<SpriteRenderer>();
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.sprite = Sprite.Create(texture2D, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f,0.5f), 100f, 0, SpriteMeshType.FullRect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlane()
    {
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        rect.width  = Math.Min(rect.width,  Screen.width  - Math.Abs((int)rect.x));
        rect.height = Math.Min(rect.height, Screen.height - Math.Abs((int)rect.y));
        texture2D.Resize((int)rect.width, (int)rect.height);
        texture2D.ReadPixels(rect, 0, 0, false);
        texture2D.Apply();
        renderer.size = new Vector2(rect.width / 100f, rect.height / 100f);
        transform.localScale = new Vector3(size.x * 100f / rect.width, size.y * 100f / rect.height, transform.localScale.z);

        RenderTexture.active = currentActiveRT;
    }
}
