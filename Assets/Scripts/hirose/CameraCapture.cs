using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCapture : MonoBehaviour
{
    //ゲーム開始時にカメラの画像を切り取って保存し、デスクトップに保存する。

    public Camera targetCamera;   // 切り取りたい2Dカメラ
    public int width = 1920;
    public int height = 1080;

    private void Start()
    {

    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame) //Cボタンで撮影
        {
            CaptureCameraView();
        }
    }

    public void CaptureCameraView()
    {
        // 一時的なRenderTextureを作成
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        targetCamera.targetTexture = renderTexture;

        // カメラに実際に描画させる
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        targetCamera.Render();

        // アクティブなRenderTextureから読み取る
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // カメラの設定を元に戻す
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // PNGとして保存
        byte[] bytes = screenshot.EncodeToPNG();

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string fileName = "capture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string path = Path.Combine(desktopPath, fileName);

        File.WriteAllBytes(path, bytes);

        Debug.Log("保存先: " + path);
    }
}