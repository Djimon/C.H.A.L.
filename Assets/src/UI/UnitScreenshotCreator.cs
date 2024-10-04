using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScreenshotCreator : MonoBehaviour
{

    public Camera screenshotCam;
    public Transform modelSpawnPoint;

    public Sprite CreateScreenshot(GameObject unitModel, string screenshotName)
    {
        // Instanziere das Modell an der richtigen Position
        GameObject modelInstance = Instantiate(unitModel, modelSpawnPoint.position, Quaternion.identity);

        // Stelle sicher, dass die Kamera auf das Modell schaut
        screenshotCam.transform.LookAt(modelInstance.transform);

        // Screenshot als Texture2D aufnehmen
        RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        screenshotCam.targetTexture = renderTexture;
        screenshotCam.Render();

        // Konvertiere die RenderTexture in eine Texture2D
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(256, 256, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        screenshot.Apply();

        // Speichere den Screenshot als PNG (oder konvertiere zu Sprite)
        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/Units/screenshots/" + screenshotName + ".png", bytes);

        //creenshot als Sprite verwenden
        Sprite unitSprite = Sprite.Create(screenshot, new Rect(0, 0, 256, 256), Vector2.one * 0.5f);

        // Zerstöre das Modell nach dem Screenshot
        DestroyImmediate(modelInstance);

        // RenderTexture aufräumen
        screenshotCam.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(renderTexture);

        return unitSprite;
    }
}
