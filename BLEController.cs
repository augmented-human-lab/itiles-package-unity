using UnityEngine;
using System;
using ITiles;
using System.Text;

public class BLEController : MonoBehaviour
{
    private AndroidJavaObject bleManager;

    #region Subscribable itile events

    public delegate void DataReceivedEventHandler(string value);
    public event DataReceivedEventHandler DataReceived;

    public delegate void DataReceivedDecomposedEventHandler(ITileMessage iTileMessage);
    public event DataReceivedDecomposedEventHandler DataReceivedDecomposed;

    public delegate void ITilesIDsDiscoveredEventHandler(string devices);
    public event ITilesIDsDiscoveredEventHandler ITilesIDsDiscovered;

    public delegate void ConnectionStateChangedEventHandler(int connectionState);
    public event ConnectionStateChangedEventHandler ConnectionStateChanged;

    #endregion

    void Start()
    {
        try
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass(CONFIG_STRINGS.UNITY_PLAYER_CLASS))
            {
                AndroidJavaObject unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
                bleManager = new AndroidJavaObject(CONFIG_STRINGS.ANDROID_LIBRARY_MAIN_CLASS, unityActivity);
                BLEDataCallbackProxy dataCallback = new BLEDataCallbackProxy(this);
                bleManager.Call(ANDROID_ITILE_METHOD.SET_CALLBACK, dataCallback);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }     
    }
    #region Event Invoker methods

    public void OnConnectionStateChanged(int connectionState) {
        ConnectionStateChanged?.Invoke(connectionState);
    }

    public void DiscoveredITilesIDs(string deviceIds) {
        ITilesIDsDiscovered?.Invoke(deviceIds);
    }

    public void ReceiveData(string value) {
        DataReceived?.Invoke(value);
        DataReceivedDecomposed?.Invoke(ReadMessage(value));
    }

    #endregion

    #region Main iTile methods

    public void StartScan() {
        bleManager.Call(ANDROID_ITILE_METHOD.START_SCAN);
    }

    public void StopScan()
    {
        bleManager.Call(ANDROID_ITILE_METHOD.STOP_SCAN);
    }

    public void Connect(string deviceAddress)
    {
        bleManager.Call(ANDROID_ITILE_METHOD.CONNECT, 
            deviceAddress, 
            CONFIG_STRINGS.ITILES_BLE_SERVICE_UUID, 
            CONFIG_STRINGS.CHARACTERISTIC_UUID_RX, 
            CONFIG_STRINGS.CHARACTERISTIC_UUID_TX
        );
    }

    public void Read()
    {
        bleManager.Call(ANDROID_ITILE_METHOD.START_READ);
    }

    public void Write(byte[] data)
    {
        bleManager.Call(ANDROID_ITILE_METHOD.WRITE, data);
    }

    private void SendRXCommand(byte command) {
        // Command packet format: [Start Byte][Command][Length][End Byte]
        byte[] commandPacket = new byte[4];
        commandPacket[0] = TX_COMMAND.START_BYTE;
        commandPacket[1] = command;
        commandPacket[2] = 4;
        commandPacket[3] = TX_COMMAND.END_BYTE;
        sbyte[] sbyteCmd = new sbyte[commandPacket.Length];
        for (int i = 0; i < commandPacket.Length; i++)
        {
            sbyteCmd[i] = (sbyte)commandPacket[i];
        }
        bleManager.Call("write", sbyteCmd);
    }

    // Method to send the specific command with parameters to the BLE device
    private void SendCommand(byte command, byte[] parameters, byte tileId = 0x00)
    {
        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]
        byte[] commandPacket = new byte[5 + parameters.Length];
        commandPacket[0] = TX_COMMAND.START_BYTE;
        commandPacket[1] = tileId;
        commandPacket[2] = command;
        commandPacket[3] = (byte)parameters.Length;
        Array.Copy(parameters, 0, commandPacket, 4, parameters.Length);
        commandPacket[^1] = TX_COMMAND.END_BYTE;

        // Convert the byte array to sbyte array
        sbyte[] sbyteCmd = new sbyte[commandPacket.Length];
        for (int i = 0; i < commandPacket.Length; i++)
        {
            sbyteCmd[i] = (sbyte)commandPacket[i];
        }

        // Send the command packet to the BLE device
        bleManager.Call("write", sbyteCmd);
    }

    // Method to decompose received command from the BLE device
    private ITileMessage ReadMessage(string message) {

        ITileMessage iTileMessage = new ITileMessage();
        Debug.Log("SANKHA UNITY >>>> " + message);
        byte[] byteMessage = HexStringToByteArray(message);

        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]
        iTileMessage.startByte = byteMessage[0];
        iTileMessage.tileId = byteMessage[1];
        iTileMessage.command = byteMessage[2];

        iTileMessage.parameters =  new byte[byteMessage.Length - 3];
        Array.Copy(byteMessage, 3, iTileMessage.parameters, 0, iTileMessage.parameters.Length);

        switch (iTileMessage.command) {
            case (byte)RX_COMMAND.REPLY_PAIRED_TILES:
                ReplyPairedTiles(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.REPLY_ONLINE_TILES:
                ReplyOnlineTiles(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.SHAKE:
                AwaitShake(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.SIDE_UPDATE:
                AwaitShake(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.STEP_CHANGE:

                break;
            case (byte)RX_COMMAND.TILE_TIMEOUT:

                break;
            case (byte)RX_COMMAND.TOUCH:
                AwaitTouch(iTileMessage.tileId, iTileMessage.parameters);
                break;
        }

        return iTileMessage;
    }

    // Method to send the BROADCAST command with the MASTER tile mac address
    public void BroadcastCommand(byte[] masterTileMacAddress)
    {
        byte command = TX_COMMAND.BROADCAST;
        //byte[] parameters = new byte[6];
        //Array.Copy(masterTileMacAddress, parameters, 6);
        SendCommand(command, masterTileMacAddress);
    }

    // Method to send the REQUEST_TILE_ID command with the STANDARD tile mac address
    public void RequestTileID(byte[] standardTileMacAddress)
    {
        //byte[] parameters = new byte[6];
        //Array.Copy(standardTileMacAddress, parameters, 6);
        SendCommand(TX_COMMAND.REQUEST_TILE_ID, standardTileMacAddress);
    }

    // Method to send the ASSIGN_ID command with the tile ID to a STANDARD tile
    public void AssignTileID(byte tileID)
    {
        SendCommand(TX_COMMAND.ASSIGN_ID, new byte[] { tileID });
    }

    // Method to send the UNPAIR command
    public void UnpairTile(byte tileID)
    {
        SendCommand(TX_COMMAND.UNPAIR, new byte[] { tileID });
    }

    public void ReplyPairedTiles(byte tileId, byte[] parameters)
    {
        Debug.Log("NO OF TILES PAIRED: " + Convert.ToInt32(parameters[0]));
        Debug.Log("They are..");
        for (int i = 1; i < parameters.Length; i++) {
            Debug.Log("Tile: " + i);
        }
        //SendCommand(RX_COMMAND.REPLY_PAIRED_TILES, new byte[0], SELECT_ITILE.MASTER);
    }

    public void ReplyOnlineTiles(byte tileId, byte[] parameters)
    {
        //SendCommand(RX_COMMAND.REPLY_ONLINE_TILES, new byte[0], SELECT_ITILE.ALL);
    }

    // Method to send the QUERY_PAIRED_TILES command
    public void QueryPairedTiles()
    {
        SendCommand(TX_COMMAND.QUERY_PAIRED_TILES, new byte[0]);
    }

    // Method to send the QUERY_ONLINE_TILES command
    public void QueryOnlineTiles()
    {
        SendCommand(TX_COMMAND.QUERY_ONLINE_TILES, new byte[0]);
    }

    // Method to send the TRIGGER_LIGHT command
    public void TriggerLight(byte redIntensity, byte greenIntensity, byte blueIntensity, byte offAfterSeconds, byte logReactionTime, byte timeoutResponse, byte tileId = 0x00)
    {
        byte[] parameters = new byte[] { redIntensity, greenIntensity, blueIntensity, offAfterSeconds, logReactionTime, timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_LIGHT, parameters, tileId);
    }

    public void TriggerLight(byte[] colorIntensities, byte offAfterSeconds, byte logReactionTime, byte timeoutResponse, byte tileId = 0x00)
    {
        if (colorIntensities.Length != 3)
        {
            throw new ArgumentException("colorIntensities array must have exactly 3 elements.");
        }
        byte[] parameters = new byte[] { colorIntensities[0], colorIntensities[1], colorIntensities[2], offAfterSeconds, logReactionTime, timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_LIGHT, parameters, tileId);
    }

    // Method to send the TRIGGER_SOUND command
    public void TriggerSound(byte soundTrackID, byte repeatCount, byte logReactionTime, byte timeoutResponse, byte tileId = 0x00)
    {
        byte[] parameters = new byte[] { soundTrackID, repeatCount, logReactionTime, timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_SOUND, parameters, tileId);
    }

    // Method to send the TRIGGER_VIBRATE command
    public void TriggerVibrate(byte vibrationPatternID, byte repeatCount, byte logReactionTime, byte timeoutResponse, byte tileId = 0x00)
    {
        byte[] parameters = new byte[] { vibrationPatternID, repeatCount, logReactionTime, timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_VIBRATE, parameters, tileId);
    }

    // Method to send the TRIGGER_SIDE command
    public void TriggerSide(byte[] sideColors, byte offAfterSeconds, byte logReactionTime, byte timeoutResponse, byte tileId = 0x00)
    {
        byte[] parameters = new byte[sideColors.Length + 3];
        Array.Copy(sideColors, 0, parameters, 0, sideColors.Length);
        parameters[^3] = offAfterSeconds;
        parameters[^2] = logReactionTime;
        parameters[^1] = timeoutResponse;
        SendCommand(TX_COMMAND.TRIGGER_SIDE, parameters, tileId);
    }

    // Method to stop light effect on the tile
    public void StopLightEffect(byte tileId = 0x00)
    {
        SendCommand(TX_COMMAND.STOP_EFFECT, new byte[0], tileId);
    }

    /// <summary>
    /// Notify app that a tile has been touched
    /// </summary>
    /// <param name="fromTileId">ID of tile that has been touched</param>
    /// <param name="reactionTimeHigh">Reaction Time (High Byte)</param>
    /// <param name="reactionTimeLow">Reaction Time (Low Byte)</param>
    public void AwaitTouch(byte tileId, byte[] parameters) {
        Debug.Log("TILE HAS BEEN TOUCHED...");
        //byte[] parameters = new byte[] {
        //    fromTileId,
        //    reactionTimeHigh,
        //    reactionTimeLow
        //};
        //SendRXCommand(RX_COMMAND.TOUCH, parameters);
    }

    /// <summary>
    /// Notify app that a tile been shaked
    /// </summary>
    /// <param name="reactionTimeHigh">Reaction Time (High Byte)</param>
    /// <param name="reactionTimeLow">Reaction Time (Low Byte)</param>
    public void AwaitShake(byte tileId, byte[] parameters)
    {
        Debug.Log("TILE HAS BEEN SHAKED...");

        //byte[] parameters = new byte[] {
        //    reactionTimeHigh,
        //    reactionTimeLow
        //};
        //SendCommand(RX_COMMAND.SHAKE, parameters);
    }

    public void SuperTrigger(byte tileId, byte[] parameters) { 
    // tx 
    }

    public void TileTimeout(byte tileId, byte[] parameters) { 
    // rx
    }

    public void ToggleAcceleration(byte tileId, byte[] parameters) { 
    // tx
    }

    public void SetAccelerationThreshold(byte tileId, byte[] parameters) { 
    // tx
    }

    public void ToggleTouchSensor(byte tileId, byte[] parameters) { 
    // tx
    }

    public void SetVolume(byte tileId, byte[] parameters) { 
    // tx
    }

    public void StopEffect(byte tileId, byte[] parameters) { 
    // tx
    }

    #endregion

    #region Utility methods
    public static byte[] HexStringToByteArray(string hex)
    {
        Debug.Log(hex);
        hex = hex.Replace(":", "");
        if (hex.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string length must be even.");
        }

        byte[] byteArray = new byte[hex.Length / 2];

        for (int i = 0; i < byteArray.Length; i++)
        {
            string byteValue = hex.Substring(i * 2, 2);
            byteArray[i] = Convert.ToByte(byteValue, 16);
        }
        return byteArray;
    }

    public static string ByteArrayToHexString(byte[] bytes) {
        foreach (byte b in bytes) {
            Debug.LogWarning(b);
        }
        return Encoding.UTF8.GetString(bytes);
    }
    #endregion

}
