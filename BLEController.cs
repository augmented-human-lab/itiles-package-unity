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

    // Method to send the specific command with parameters to the BLE device
    private void SendCommand(TX_COMMAND command, byte[] parameters, SELECT_ITILE tileId = SELECT_ITILE.MASTER)
    {
        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]
        byte[] commandPacket = new byte[5 + parameters.Length];
        commandPacket[0] = (byte)TX_COMMAND.START_BYTE;
        commandPacket[1] = (byte)tileId;
        commandPacket[2] = (byte)command;
        commandPacket[3] = (byte)parameters.Length;
        Array.Copy(parameters, 0, commandPacket, 4, parameters.Length);
        commandPacket[^1] = (byte)TX_COMMAND.END_BYTE;

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
            case (byte)RX_COMMAND.REQUEST_TILE_ID:
                OnRequestTileID(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.REPLY_PAIRED_TILES:
                OnReplyPairedTiles(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.REPLY_ONLINE_TILES:
                OnReplyOnlineTiles(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.SHAKE:
                OnTileShake(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.SIDE_UPDATE:
                OnSideUpdate(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.STEP_CHANGE:
                OnStepChange(iTileMessage.tileId, iTileMessage.parameters);
                break;
            case (byte)RX_COMMAND.TILE_TIMEOUT:
                OnTileTimeout();
                break;
            case (byte)RX_COMMAND.TOUCH:
                OnTileTouch(iTileMessage.tileId, iTileMessage.parameters);
                break;
        }

        return iTileMessage;
    }

    // Method to send the BROADCAST command with the MASTER tile mac address
    public void BroadcastCommand(byte[] masterTileMacAddress)
    {
        SendCommand(TX_COMMAND.BROADCAST, masterTileMacAddress);
    }

    // Method to send the REQUEST_TILE_ID command with the STANDARD tile mac address
    public void OnRequestTileID(byte tileId, byte[] standardTileMacAddress)
    {
        Debug.Log("Tile ID: " + tileId + " is requesting for a tile id to be assigned");
        string mac = "";
        for (int i = 1; i < standardTileMacAddress.Length; i++)
        {
            mac += i + ":";
        }
        Debug.Log("Their Mac address: " + mac);
    }

    // Method to send the ASSIGN_ID command with the tile ID to a STANDARD tile
    public void AssignTileID(byte tileID)
    {
        if (tileID < 0x01 || tileID > 0x7f) {
            throw new ArgumentException("Standard tile Id must be within 0x01 to 0x7f");
        }
        SendCommand(TX_COMMAND.ASSIGN_ID, new byte[] { tileID });
    }

    // Method to send the UNPAIR command
    public void UnpairTile(SELECT_ITILE tileID)
    {
        SendCommand(TX_COMMAND.UNPAIR, new byte[] { (byte)tileID });
    }

    public void OnReplyPairedTiles(byte tileId, byte[] parameters)
    {
        Debug.Log("NO OF TILES PAIRED: " + Convert.ToInt32(parameters[0]));
        Debug.Log("They are..");
        for (int i = 1; i < parameters.Length; i++) {
            Debug.Log("Tile: " + i);
        }
    }

    public void OnReplyOnlineTiles(byte tileId, byte[] parameters)
    {
        Debug.Log("Tile online: " + tileId);
        Debug.Log("Battary: " + Convert.ToInt32(parameters[0]));
        Debug.Log("Hardware version: " + Convert.ToInt32(parameters[1]));
        Debug.Log("Firmware version: " + Convert.ToInt32(parameters[2]));
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

    public void TriggerLight(
        byte[] colorIntensities, 
        TIMEOUT_DELAY offAfterSeconds, 
        LOG_REACTION_TIME logReactionTime, 
        TIMEOUT_RESPONSE timeoutResponse, 
        SELECT_ITILE tileId
    )
    {
        if (colorIntensities.Length != 3)
        {
            throw new ArgumentException("colorIntensities array must have exactly 3 elements.");
        }
        byte[] parameters = new byte[] { colorIntensities[0], colorIntensities[1], colorIntensities[2], (byte)offAfterSeconds, (byte)logReactionTime, (byte)timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_LIGHT, parameters, tileId);
    }

    // Method to send the TRIGGER_SOUND command
    public void TriggerSound(
        byte soundTrackID, 
        REPEAT_COUNT repeatCount, 
        LOG_REACTION_TIME logReactionTime, 
        TIMEOUT_RESPONSE timeoutResponse, 
        SELECT_ITILE tileId
    )
    {
        byte[] parameters = new byte[] { soundTrackID, (byte)repeatCount, (byte)logReactionTime, (byte)timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_SOUND, parameters, tileId);
    }

    // Method to send the TRIGGER_VIBRATE command
    public void TriggerVibrate(
        VIBRATION_PATTERN vibrationPatternID, 
        REPEAT_COUNT repeatCount, 
        LOG_REACTION_TIME logReactionTime, 
        TIMEOUT_RESPONSE timeoutResponse, 
        SELECT_ITILE tileId)
    {
        byte[] parameters = new byte[] { (byte)vibrationPatternID, (byte)repeatCount, (byte)logReactionTime, (byte)timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_VIBRATE, parameters, tileId);
    }

    // Method to send the TRIGGER_SIDE command
    public void TriggerSide(
        byte[] sideColors, 
        TIMEOUT_DELAY offAfterSeconds, 
        LOG_REACTION_TIME logReactionTime, 
        TIMEOUT_RESPONSE timeoutResponse, 
        SELECT_ITILE tileId
    )
    {
        if (sideColors.Length != 3)
        {
            throw new ArgumentException("sideColors array must have exactly 18 elements.");
        }
        byte[] parameters = new byte[sideColors.Length + 3];
        Array.Copy(sideColors, 0, parameters, 0, sideColors.Length);
        parameters[^3] = (byte)offAfterSeconds;
        parameters[^2] = (byte)logReactionTime;
        parameters[^1] = (byte)timeoutResponse;
        SendCommand(TX_COMMAND.TRIGGER_SIDE, parameters, tileId);
    }

    // Method to stop light effect on the tile
    public void StopLightEffect(SELECT_ITILE tileId)
    {
        SendCommand(TX_COMMAND.STOP_EFFECT, new byte[0], tileId);
    }

    /// <summary>
    /// Notify app that a tile has been touched
    /// </summary>
    /// <param name="fromTileId">ID of tile that has been touched</param>
    /// <param name="reactionTimeHigh">Reaction Time (High Byte)</param>
    /// <param name="reactionTimeLow">Reaction Time (Low Byte)</param>
    public void OnTileTouch(byte tileId, byte[] parameters) {
        Debug.Log("TILE HAS BEEN TOUCHED...");
        Debug.Log("Which Tile: " + parameters[0]);
        int reactionTime = (parameters[1] << 8) | parameters[2];
        Debug.Log("Reaction time: " + reactionTime);
    }

    /// <summary>
    /// Notify app that a tile been shaked
    /// </summary>
    /// <param name="reactionTimeHigh">Reaction Time (High Byte)</param>
    /// <param name="reactionTimeLow">Reaction Time (Low Byte)</param>
    public void OnTileShake(byte tileId, byte[] parameters)
    {
        Debug.Log("TILE HAS BEEN SHAKED...");
        Debug.Log("Which Tile: " + parameters[0]);
        int reactionTime = (parameters[1] << 8) | parameters[2];
        Debug.Log("Reaction time: " + reactionTime);
    }

    public void SuperTrigger(byte[] parameters, SELECT_ITILE tileId) {
        SendCommand(TX_COMMAND.SUPER_TRIGGER, parameters, tileId);
    }

    public void OnTileTimeout() {
        Debug.Log("A TILE HAS BEEN Timed out...");
    }

    public void OnSideUpdate(byte tileId, byte[] parameters) {
        Debug.Log("A TILE SIDE HAS BEEN UPDATED");
        Debug.Log("Which Tile: " + tileId);
        Debug.Log("which side: " + parameters[0]);
        Debug.Log("Unpair or pair: " + parameters[1]);
        int reactionTime = (parameters[2] << 8) | parameters[3];
        Debug.Log("Reaction time: " + reactionTime);
    }

    public void OnStepChange(byte tileId, byte[] parameters) {
        Debug.Log("Someone has stepped on or off tile");
        Debug.Log("Which Tile: " + tileId);
        Debug.Log("Step on or off: " + parameters[0]);
        int reactionTime = (parameters[1] << 8) | parameters[2];
        Debug.Log("Reaction time: " + reactionTime);
    }

    public void ToggleAcceleration(byte[] parameters, SELECT_ITILE tileId) {
        SendCommand(TX_COMMAND.ENABLE_DISABLE_ACCEL, parameters, tileId);
    }

    public void SetAccelerationThreshold(byte[] parameters, SELECT_ITILE tileId) {
        SendCommand(TX_COMMAND.SET_ACCEL_THRESHOLD, parameters, tileId);
    }

    public void ToggleTouchSensor(byte[] parameters, SELECT_ITILE tileId) {
        SendCommand(TX_COMMAND.ENABLE_DISABLE_TOUCH, parameters, tileId);
    }

    public void SetVolume(byte[] parameters, SELECT_ITILE tileId) {
        throw new NotImplementedException();
    }

    public void StopEffect(byte[] parameters, SELECT_ITILE tileId) {
        SendCommand(TX_COMMAND.STOP_EFFECT, parameters, tileId);
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
