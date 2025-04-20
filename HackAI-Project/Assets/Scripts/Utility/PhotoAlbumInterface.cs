using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class PhotoAlbumInterface : MonoBehaviour
{
    public RawImage profilePreview; // Assign in Inspector
    public Texture2D profileTexture;

    void Awake()
    {
        profileTexture = profilePreview.texture as Texture2D;   
    }

    public void PickImageFromGallery() {
        // Prompt to get an image from the user's gallary
        NativeGallery.GetImageFromGallery(path => {
            profileTexture = LoadTextureFromFile(path);
            profilePreview.texture = profileTexture;
        }, title: "Profile Picture", mime: "image/*" );
    }

    public Texture2D LoadTextureFromFile(string path, int maxSize = 512)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("File does not exist at path: " + path);
            return null;
        }

        byte[] imageData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2); // Placeholder size, will auto-resize on LoadImage

        if (texture.LoadImage(imageData))
        {
            // Optionally resize if needed here
            return texture;
        }

        Debug.LogError("Failed to load image from data.");
        return null;
    }
}