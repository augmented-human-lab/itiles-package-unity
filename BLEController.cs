using UnityEngine;
using System;

public class BLEController : MonoBehaviour
{
    private AndroidJavaObject bleManager;

    public delegate void DataReceivedEventHandler(string value);
    public event DataReceivedEventHandler DataReceived;

    void Start()
    {
        try
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
                bleManager = new AndroidJavaObject("org.ahlab.itiles.plugin.BLEManager", unityActivity);
                BLEDataCallbackProxy dataCallback = new BLEDataCallbackProxy(this);
                bleManager.Call("setDataCallback", dataCallback);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }     
    }

    public void ReceiveData(string value) {
        DataReceived?.Invoke(value);
    }

    public void SearchITiles() {
        bleManager.Call("searchITiles");
    }

    public void StopSearchingITiles()
    {
        bleManager.Call("stopSearchingITiles");
    }

    public bool Connect(string deviceAddress, string serviceUUID, string characteristicUUIDRx, string characteristicUUIDTx)
    {
        return bleManager.Call<bool>("connect", deviceAddress, serviceUUID, characteristicUUIDRx, characteristicUUIDTx);
    }

    public void Read()
    {
        bleManager.Call("startReadingDataStream");
    }

    public void Write(byte[] data)
    {
        bleManager.Call("write", data);
    }

    public void SendTestCommand() {
        byte[] triggerVibrateCommand = new byte[]{
            (byte) 0xAA, // Start Byte
            (byte) 0x01, // Tile ID (0x01 for a specific tile ID)
            (byte) 0x0D, // Command: TRIGGER_VIBRATE (0x0D)
            (byte) 0x04, // Length (Number of bytes for Parameters)
            (byte) 0x03, // Parameter 1: Vibration pattern ID (0x01 - 0xFF)
            (byte) 0x01, // Parameter 2: Repeat 1 time
            (byte) 0x03, // Parameter 3: Log reaction time for both touch/step and shake
            (byte) 0x0A, // Parameter 4: Timeout response after 10 seconds if not touched/shake
            (byte) 0xEF // End Byte
        };

        sbyte[] sbyteCmd = new sbyte[triggerVibrateCommand.Length];
        for (int i = 0; i < triggerVibrateCommand.Length; i++)
        {
            sbyteCmd[i] = (sbyte)triggerVibrateCommand[i];
        }

        // Send the command packet to the BLE device
        bleManager.Call("write", sbyteCmd);
    }

    // Method to send the specific command with parameters to the BLE device
    public void SendCommand(byte command, byte[] parameters)
    {
        // Command packet format: [Start Byte][Command][Length][Parameters][End Byte]
        byte[] commandPacket = new byte[5 + parameters.Length];
        commandPacket[0] = 0x7E; // Start Byte
        commandPacket[1] = 0x00; // tile id
        commandPacket[2] = command;
        commandPacket[3] = (byte)parameters.Length;

        Array.Copy(parameters, 0, commandPacket, 4, parameters.Length);

        string hexString = BitConverter.ToString(commandPacket).Replace("-", " ");
        Debug.Log("Byte array as hexadecimal string: " + hexString);

        commandPacket[commandPacket.Length - 1] = 0xEF; // End Byte

        // Convert the byte array to sbyte array
        sbyte[] sbyteCmd = new sbyte[commandPacket.Length];
        for (int i = 0; i < commandPacket.Length; i++)
        {
            sbyteCmd[i] = (sbyte)commandPacket[i];
        }

        // Send the command packet to the BLE device
        bleManager.Call("write", sbyteCmd);
    }

    // Method to send the BROADCAST command with the MASTER tile mac address
    public void BroadcastCommand(byte[] masterTileMacAddress)
    {
        byte command = 0x01;
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


}
