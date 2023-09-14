using UnityEngine;

public class BLEDataCallbackProxy : AndroidJavaProxy, BLEDataCallback
{
    private BLEController targetMonoBehaviour;

    public BLEDataCallbackProxy(BLEController target) : base("org.ahlab.itiles.plugin.BLEDataCallback") {
        targetMonoBehaviour = target;
    }

    public void onDataReceived(string value)
    {
        targetMonoBehaviour.ReceiveData(value);
    }
}
