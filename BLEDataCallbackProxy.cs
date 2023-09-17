using ITiles;
using UnityEngine;

public class BLEDataCallbackProxy : AndroidJavaProxy, BLEDataCallback
{
    private readonly BLEController targetMonoBehaviour;

    public BLEDataCallbackProxy(BLEController target) : base(CONFIG_STRINGS.ANDROID_UNITY_CALLBACK_INTERFACE) {
        targetMonoBehaviour = target;
    }

    public void onDataReceived(string value)
    {
        targetMonoBehaviour.ReceiveData(value);
    }

    public void onITilesIDsDiscovered(string deviceIds)
    {
        targetMonoBehaviour.DiscoveredITilesIDs(deviceIds);
    }

    public void onConnectionStateChanged(int connectionState) {
        targetMonoBehaviour.OnConnectionStateChanged(connectionState);
    }

}
