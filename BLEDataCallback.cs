public interface BLEDataCallback
{
    void onDataReceived(string value);
    void onITilesIDsDiscovered(string deviceIds);
    void onConnectionStateChanged(int connectionState);
}
