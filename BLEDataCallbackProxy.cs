using UnityEngine;

public class BLEDataCallbackProxy : AndroidJavaProxy, BLEDataCallback
{
    public BLEDataCallbackProxy() : base("org.ahlab.itiles.plugin.BLEDataCallback") { }

    public void onDataReceived(string value)
    {
        Debug.Log("Received value from Android: " + value);

        // Do something with the received value in Unity
    }
}
