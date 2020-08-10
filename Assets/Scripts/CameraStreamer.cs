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

    private RenderTexture rt = null;

    private Texture2D texture = null;

    private Rect rect;

    void Start()
    {
        frame = new CapturedFrame
        {
            encodedData = ""
        };

        wsService.AddWebSocketService<StreamingService>("/screen_streaming", (service) => service.frame = frame);
    }

    private void OnPostRender()
    {
        if (RenderTexture.active == null) 
        {
            return;
        }

        if (rt == null) 
        {
            rt = RenderTexture.active;

            texture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

            rect = new Rect(0, 0, rt.width, rt.height);
        }

        texture.ReadPixels(rect, 0, 0, false);
        texture.Apply();

        frame.encodedData = Convert.ToBase64String(texture.EncodeToPNG());
    }
 }
