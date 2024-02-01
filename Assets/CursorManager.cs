using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Texture2D bloodCursor;
    [SerializeField] private Texture2D spiritCursor;

    private Texture2D currentTexture;
    private bool isStatic;

    // Start is called before the first frame update
    void Awake()
    {
        if (player != null)
        {
            player.AddChangeFormCallback(OnPlayerFormChanged);
            OnPlayerFormChanged();
            isStatic = false;
        }
        else
        {
            int tex = Random.Range(0, 2);
            if (tex == 0)
            {
                currentTexture = spiritCursor;
            }
            else
            {
                currentTexture = bloodCursor;
            }
            Texture2D cursorTex = RotatedImage(currentTexture, 120);
            Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.ForceSoftware);
            isStatic = true;
        }
    }

    void OnPlayerFormChanged()
    {
        if (player.IsSpirit())
        {
            currentTexture = spiritCursor;
        }
        else
        {
            currentTexture = bloodCursor;
        }
    }

    private void Update()
    {
        if (!isStatic)
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = cursorPos - (Vector2)player.transform.position;
            float angle = Vector2Extensions.Angle360(Vector2.right, dir);
            Texture2D cursorTex = RotatedImage(currentTexture, angle);
            Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.ForceSoftware);
        }
    }

    public Texture2D RotatedImage(Texture2D tex, float angleDegrees)
    {
        int width = tex.width;
        int height = tex.height;
        float halfHeight = height * 0.5f;
        float halfWidth = width * 0.5f;
        Texture2D rotatedTex = new(width, height, TextureFormat.RGBA32, true);
        //rotatedTex.alphaIsTransparency = true;

        var oldTexels = tex.GetRawTextureData<Color32>();
        var newTexels = rotatedTex.GetRawTextureData<Color32>();
        var copy = System.Buffers.ArrayPool<Color32>.Shared.Rent(oldTexels.Length);
        Unity.Collections.NativeArray<Color32>.Copy(oldTexels, copy, oldTexels.Length);

        float phi = Mathf.Deg2Rad * angleDegrees;
        float cosPhi = Mathf.Cos(phi);
        float sinPhi = Mathf.Sin(phi);

        int address = 0;
        for (int newY = 0; newY < height; newY++)
        {
            for (int newX = 0; newX < width; newX++)
            {
                float cX = newX - halfWidth;
                float cY = newY - halfHeight;
                int oldX = Mathf.RoundToInt(cosPhi * cX + sinPhi * cY + halfWidth);
                int oldY = Mathf.RoundToInt(-sinPhi * cX + cosPhi * cY + halfHeight);
                bool InsideImageBounds = (oldX > -1) & (oldX < width)
                                       & (oldY > -1) & (oldY < height);

                newTexels[address++] = InsideImageBounds ? copy[oldY * width + oldX] : default;
            }
        }

        // No need to reinitialize or SetPixels - data is already in-place.
        rotatedTex.Apply(true);

        System.Buffers.ArrayPool<Color32>.Shared.Return(copy);
        return rotatedTex;
    }
}
