using System;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public struct ControlMessage
{
    public string action;
}

public class RemoteInput {
    public float throttle;
    public DateTime throttleLastUpdate = DateTime.UtcNow;
    public float steer;
    public DateTime steerLastUpdate = DateTime.UtcNow;
}

public class RemoteControlService : WebSocketBehavior
{
    public RemoteInput ri;

    private TimeSpan nextReceiveDelta = TimeSpan.FromSeconds(1.0 / 40.0);

    private DateTime nextReceive = DateTime.UtcNow;

    protected override void OnMessage(MessageEventArgs e)
    {
        

        var now = DateTime.UtcNow;

        if (now < nextReceive)
        {
            return;
        }

        base.OnMessage(e);

        nextReceive += nextReceiveDelta;

        ControlMessage msg = JsonUtility.FromJson<ControlMessage>(e.Data.ToString());


        switch (msg.action)
        {
            case "left":
                {
                    ri.steer = Mathf.Max(-1.0f, ri.steer - 0.1f);
                    ri.steerLastUpdate = now;
                    break;
                }
            case "right":
                {
                    ri.steer = Mathf.Min(1.0f, ri.steer + 0.1f);
                    ri.steerLastUpdate = now;
                    break;
                }
            case "forward":
                {
                    ri.throttle = Mathf.Min(1.0f, ri.throttle + 0.05f);
                    ri.throttleLastUpdate = now;
                    break;
                }
            case "backward":
                {
                    ri.throttle = Mathf.Max(-1.0f, ri.throttle - 0.05f);
                    ri.throttleLastUpdate = now;
                    break;
                }
        }
    }
}
public class InputController : MonoBehaviour
{
    public string inputSteerAxis = "Horizontal";
    public string inputThrottleAxis = "Vertical";

    public WebsocketService wsService;

    public float ThrottleInput { get; private set; }
    public float SteerInput { get; private set; }

    private RemoteInput ri = new RemoteInput();

    private void Start()
    {
        wsService.AddWebSocketService<RemoteControlService>("/remote_control", (service) => service.ri = ri);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SteerInput = Input.GetAxis(inputSteerAxis) + ri.steer;
        ThrottleInput = Input.GetAxis(inputThrottleAxis) + ri.throttle;

        var now = DateTime.UtcNow;
        float throttleLastUpdateDelta = (now - ri.throttleLastUpdate).Seconds;
        float steerLastUpdateDelta = (now - ri.steerLastUpdate).Seconds;
        float maxInterval = Time.fixedDeltaTime * 3;


        if (steerLastUpdateDelta > maxInterval)
        {
            if (ri.steer > 0)
            {
                ri.steer = Mathf.Max(0, ri.steer - 0.1f);
            }
            else if (ri.steer < 0)
            {
                ri.steer = Mathf.Min(0, ri.steer + 0.1f);
            }
        }
        if (throttleLastUpdateDelta > maxInterval) 
        { 
            if (ri.throttle > 0)
            {
                ri.throttle = Mathf.Max(0, ri.throttle - 0.1f);
            }
            else if (ri.throttle < 0)
            {
                ri.throttle = Mathf.Min(0, ri.throttle + 0.1f);
            }
            
        }
        
    }

   
}
