using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEAreaPlane : MonoBehaviour
{
    new public Camera camera;
    public Vector2 size;
    // Relativ to the inside of the square
    public Vector2 vertex00;
    public Vector2 vertex01;
    public Vector2 vertex10;
    public Vector2 vertex11;

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
        float minX = Math.Min(Math.Min(vertex00.x, vertex01.x), Math.Min(vertex10.x, vertex11.x));
        float minY = Math.Min(Math.Min(vertex00.y, vertex01.y), Math.Min(vertex10.y, vertex11.y));
        float maxX = Math.Max(Math.Max(vertex00.x, vertex01.x), Math.Max(vertex10.x, vertex11.x));
        float maxY = Math.Max(Math.Max(vertex00.y, vertex01.y), Math.Max(vertex10.y, vertex11.y));
        if (maxX <= 0 || maxY <= 0 || minX >= Screen.width || minY >= Screen.height)
            return;

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        Rect rect = new Rect(minX, minY, maxX-minX, maxY-minY);
        Rect readRect = new Rect(minX, minY, Math.Min(rect.width, Screen.width-minX), maxY - (minY < 0 ? 0 : minY));
        texture2D.Resize((int)rect.width, (int)rect.height);
        texture2D.ReadPixels(readRect, (int)(minX < 0 ? -minX : 0), (int)(maxY > Screen.height ? maxY - Screen.height : 0), false);
        texture2D.Apply();
        renderer.size = new Vector2(rect.width / 100f, rect.height / 100f);
        transform.localScale = new Vector3(size.x * 100f / rect.width, size.y * 100f / rect.height, transform.localScale.z);

        RenderTexture.active = currentActiveRT;
    }
}
