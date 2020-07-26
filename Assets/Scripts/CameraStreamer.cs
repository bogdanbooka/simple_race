using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;


public class CapturedFrame {
    public string encodedData;
}

public class StreamingService : WebSocketBehavior
{
    private double frequency = 30;
    public CapturedFrame frame;

    protected override void OnOpen()
    {
        Context.WebSocket.Accept();

        Task.Run(() =>
        {
            DateTime nextSend = DateTime.UtcNow;

            while (Context.WebSocket.ReadyState == WebSocketState.Open)
            {
                var now = DateTime.UtcNow;

                if (now < nextSend)
                {
                    System.Threading.Thread.Sleep(nextSend - now);

                    continue;
                }

                nextSend += TimeSpan.FromSeconds(1f / frequency);

                Send(frame.encodedData);
            }
        });
    }

}

public class CameraStreamer : MonoBehaviour
{
    public Camera camera;

    public WebsocketService wsService;

    private CapturedFrame frame;


    void Start()
    {
        frame = new CapturedFrame
        {
            encodedData = ""
        };

        wsService.AddWebSocketService<StreamingService>("/screen_streaming", (service) => service.frame = frame);
    }

    private void LateUpdate()
    {
        StartCoroutine(TakeScreenShot());
    }

    public IEnumerator TakeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        frame.encodedData = Convert.ToBase64String(bytes);
    }

 }
