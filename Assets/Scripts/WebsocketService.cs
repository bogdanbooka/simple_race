using System;
using UnityEngine;
using WebSocketSharp.Server;

public class WebsocketService : MonoBehaviour
{
    public string ServerUri = @"ws://[::1]:8090";

    static private WebSocketServer _server = null;


    // Start is called before the first frame update
    void Awake()
    {
        if (_server == null)
        {
            WebSocketServer server = new WebSocketServer(ServerUri);

            server.Start();

            _server = server;
        }

    }

    private void OnDestroy()
    {
        _server.Stop();
    }

    public void AddWebSocketService<TBehaviorWithNew>(string path, Action<TBehaviorWithNew> initializer) where TBehaviorWithNew : WebSocketBehavior, new()
    {
        _server.AddWebSocketService<TBehaviorWithNew>(path, initializer);
    }
}
