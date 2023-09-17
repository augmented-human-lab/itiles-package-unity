using UnityEngine;
using System;
using ITiles;

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
                bleManager.Call("setDataCallback", dataCallback);
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
        bleManager.Call("startSearchingITiles");
    }

    public void StopScan()
    {
        bleManager.Call("stopSearchingITiles");
    }

    public void Connect(string deviceAddress)
    {
        bleManager.Call("connect", 
            deviceAddress, 
            CONFIG_STRINGS.ITILES_BLE_SERVICE_UUID, 
            CONFIG_STRINGS.CHARACTERISTIC_UUID_RX, 
            CONFIG_STRINGS.CHARACTERISTIC_UUID_TX
        );
    }

    public void Read()
    {
        bleManager.Call("startReadingDataStream");
    }

    public void Write(byte[] data)
    {
        bleManager.Call("write", data);
    }

    // Method to send the specific command with parameters to the BLE device
    private void SendCommand(byte command, byte[] parameters)
    {
        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]
        byte[] commandPacket = new byte[5 + parameters.Length];
        commandPacket[0] = 0x7E; // Start Byte
        commandPacket[1] = 0x00; // Tile ID Byte
        commandPacket[2] = command; // Command Byte
        commandPacket[3] = (byte)parameters.Length; // Length Byte
        Array.Copy(parameters, 0, commandPacket, 4, parameters.Length); // Parameter Bytes
        commandPacket[^1] = 0xEF; // End Byte

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
        byte[] byteMessage = HexStringToByteArray(message);
        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]
        iTileMessage.startByte = byteMessage[0];
        iTileMessage.tileId = byteMessage[1];
        iTileMessage.command = byteMessage[2];
        iTileMessage.length = byteMessage[3];
        //iTileMessage.parameters = new byte[iTileMessage.length - 5];
        //Array.Copy(byteMessage, 4, iTileMessage.parameters, 0, iTileMessage.parameters.Length);
        iTileMessage.endByte = byteMessage[^1];
        return iTileMessage;
    }

    // Method to send the BROADCAST command with the MASTER tile mac address
    public void BroadcastCommand(byte[] masterTileMacAddress)
    {
        byte command = TX_COMMAND.BROADCAST;
        byte[] parameters = new byte[6];
        Array.Copy(masterTileMacAddress, parameters, 6);

        SendCommand(command, parameters);
    }

    // Method to send the REQUEST_TILE_ID command with the STANDARD tile mac address
    public void RequestTileID(byte[] standardTileMacAddress)
    {
        byte command = 0x02;
        byte[] parameters = new byte[6];
        Array.Copy(standardTileMacAddress, parameters, 6);

        SendCommand(command, parameters);
    }

    // Method to send the ASSIGN_ID command with the tile ID to a STANDARD tile
    public void AssignTileID(byte tileID)
    {
        byte command = 0x03;
        byte[] parameters = new byte[] { tileID };

        SendCommand(command, parameters);
    }

    // Method to send the UNPAIR command
    public void UnpairTile(byte tileID)
    {
        byte command = 0x04;
        byte[] parameters = new byte[] { tileID };

        SendCommand(command, parameters);
    }

    // Method to send the QUERY_PAIRED_TILES command
    public void QueryPairedTiles()
    {
        byte command = 0x05;
        byte[] parameters = new byte[0];

        SendCommand(command, parameters);
    }

    // Method to send the QUERY_ONLINE_TILES command
    public void QueryOnlineTiles()
    {
        byte command = 0x06;
        byte[] parameters = new byte[0];

        SendCommand(command, parameters);
    }

    // Method to send the TRIGGER_LIGHT command
    public void TriggerLight(byte redIntensity, byte greenIntensity, byte blueIntensity, byte offAfterSeconds, byte logReactionTime, byte timeoutResponse)
    {
        byte command = 0x0B;
        byte[] parameters = new byte[] { redIntensity, greenIntensity, blueIntensity, offAfterSeconds, logReactionTime, timeoutResponse };

        SendCommand(command, parameters);
    }

    // Method to send the TRIGGER_SOUND command
    public void TriggerSound(byte soundTrackID, byte repeatCount, byte logReactionTime, byte timeoutResponse)
    {
        byte command = 0x0C;
        byte[] parameters = new byte[] { soundTrackID, repeatCount, logReactionTime, timeoutResponse };

        SendCommand(command, parameters);
    }

    // Method to send the TRIGGER_VIBRATE command
    public void TriggerVibrate(byte vibrationPatternID, byte repeatCount, byte logReactionTime, byte timeoutResponse)
    {
        byte command = 0x0D;
        byte[] parameters = new byte[] { vibrationPatternID, repeatCount, logReactionTime, timeoutResponse };

        SendCommand(command, parameters);
    }

    // Method to send the TRIGGER_SIDE command
    public void TriggerSide(byte[] sideColors, byte offAfterSeconds, byte logReactionTime, byte timeoutResponse)
    {
        byte command = 0x0E;
        byte[] parameters = new byte[sideColors.Length + 3];
        Array.Copy(sideColors, 0, parameters, 0, sideColors.Length);
        parameters[parameters.Length - 3] = offAfterSeconds;
        parameters[parameters.Length - 2] = logReactionTime;
        parameters[parameters.Length - 1] = timeoutResponse;

        SendCommand(command, parameters);
    }

    // Method to stop light effect on the tile
    public void StopLightEffect()
    {
        byte command = 0x1C;
        byte[] parameters = new byte[0];

        SendCommand(command, parameters);
    }

    /// <summary>
    /// Notify app that a tile has been touched
    /// </summary>
    /// <param name="fromTileId">ID of tile that has been touched</param>
    /// <param name="reactionTimeHigh">Reaction Time (High Byte)</param>
    /// <param name="reactionTimeLow">Reaction Time (Low Byte)</param>
    public void AwaitTouch(byte fromTileId, byte reactionTimeHigh, byte reactionTimeLow) {
        byte[] parameters = new byte[] {
            fromTileId,
            reactionTimeHigh,
            reactionTimeLow
        };
        SendCommand(0x12, parameters);
    }

    /// <summary>
    /// Notify app that a tile been shaked
    /// </summary>
    /// <param name="reactionTimeHigh">Reaction Time (High Byte)</param>
    /// <param name="reactionTimeLow">Reaction Time (Low Byte)</param>
    public void AwaitShake(byte reactionTimeHigh, byte reactionTimeLow)
    {
        byte[] parameters = new byte[] {
            reactionTimeHigh,
            reactionTimeLow
        };
        SendCommand(0x15, parameters);
    }
    #endregion

    #region Utility methods
    static byte[] HexStringToByteArray(string hex)
    {
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
    #endregion

}
