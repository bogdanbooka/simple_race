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
    public WebsocketService wsService;

    private CapturedFrame frame;

    public RenderTexture rt;
    private RenderTexture tmprt = null;
    private RenderTexture camtmprt = null;

    private Texture2D texture = null;

    private Rect rect;

    public Camera camera;

    void Start()
    {
        frame = new CapturedFrame
        {
            encodedData = ""
        };

        wsService.AddWebSocketService<StreamingService>("/screen_streaming", (service) => service.frame = frame);

        texture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        rect = new Rect(0, 0, rt.width, rt.height);
    }

    private void LateUpdate()
    {
        StartCoroutine(TakeScreenShot());
    }

    public IEnumerator TakeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        tmprt = RenderTexture.active;
        camtmprt = camera.targetTexture;
        camera.targetTexture = rt;
        RenderTexture.active = rt;

        camera.Render();

        // Read screen contents into the texture
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        RenderTexture.active = tmprt;
        camera.targetTexture = camtmprt;

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();

        frame.encodedData = Convert.ToBase64String(bytes);
    }
 }
