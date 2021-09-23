using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEAreaPlane : MonoBehaviour
{
    new public Camera camera;
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

    public void UpdatePlane(Matrix4x4 worldToCameraMatrix, Matrix4x4 transformMatrix)
    {
        Vector4 tmp;
        Matrix4x4 spaceToScreen = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * worldToCameraMatrix * transformMatrix;
        Vector3 topRight = transform.position + transform.right * size.x / 2 + transform.up * size.y / 2;
                tmp = spaceToScreen * new Vector4(topRight.x, topRight.y, topRight.z, 1);
                topRight = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 topLeft = transform.position - transform.right * size.x / 2 + transform.up * size.y / 2;
                tmp = spaceToScreen * new Vector4(topLeft.x, topLeft.y, topLeft.z, 1);
                topLeft = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 bottomRight = transform.position + transform.right * size.x / 2 - transform.up * size.y / 2;
                tmp = spaceToScreen * new Vector4(bottomRight.x, bottomRight.y, bottomRight.z, 1);
                bottomRight = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);
        Vector3 bottomLeft = transform.position - transform.right * size.x / 2 - transform.up * size.y / 2;
                tmp = spaceToScreen * new Vector4(bottomLeft.x, bottomLeft.y, bottomLeft.z, 1);
                bottomLeft = new Vector3((tmp.x / tmp.w + 1) / 2 * Screen.width, (tmp.y / tmp.w + 1) / 2 * Screen.height, tmp.z / tmp.w);

        float minX = Math.Min(Math.Min(topRight.x, topLeft.x), Math.Min(bottomRight.x, bottomLeft.x));
        float minY = Math.Min(Math.Min(topRight.y, topLeft.y), Math.Min(bottomRight.y, bottomLeft.y));
        float maxX = Math.Max(Math.Max(topRight.x, topLeft.x), Math.Max(bottomRight.x, bottomLeft.x));
        float maxY = Math.Max(Math.Max(topRight.y, topLeft.y), Math.Max(bottomRight.y, bottomLeft.y));
        if (maxX <= 0 || maxY <= 0 || minX >= Screen.width || minY >= Screen.height)
            return;

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.worldToCameraMatrix = worldToCameraMatrix * transformMatrix;
        camera.Render();

        Rect rect = new Rect(minX, minY, maxX-minX, maxY-minY);
        Rect readRect = new Rect(minX, minY, Math.Min(rect.width, Screen.width-minX), maxY - (minY < 0 ? 0 : minY));
        texture2D.Resize((int)rect.width, (int)rect.height);
        texture2D.ReadPixels(readRect, (int)(minX < 0 ? -minX : 0), (int)(maxY > Screen.height ? maxY - Screen.height : 0), false);
        texture2D.Apply();
        renderer.size = new Vector2(rect.width / 100f, rect.height / 100f);
        transform.localScale = new Vector3(size.x * 100f / rect.width, size.y * 100f / rect.height, transform.localScale.z);
        if (Vector3.Dot(transform.position, camera.transform.position) < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        RenderTexture.active = currentActiveRT;
    }
}
