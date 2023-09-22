using UnityEngine;
using System;
using ITiles;
using System.Text;

public class BLEController : MonoBehaviour
{
    private AndroidJavaObject bleManager;

    #region Subscribable itile events

    public delegate void DataReceivedEventHandler(string data);
    public event DataReceivedEventHandler DataReceived;

    public delegate void ITilesIDsDiscoveredEventHandler(string devices);
    public event ITilesIDsDiscoveredEventHandler ITilesIDsDiscovered;

    public delegate void ConnectionStateChangedEventHandler(int connectionState);
    public event ConnectionStateChangedEventHandler ConnectionStateChanged;


    public delegate void ITileTouchedEventHandler(TOUCH_RESPONSE touch_response);
    public event ITileTouchedEventHandler ITileTouched;

    public delegate void ITileShakedEventHandler(SHAKE_RESPONSE shake_response);
    public event ITileShakedEventHandler ITileShaked;

    public delegate void ITileSideUpdatedEventHandler(SIDE_UPDATE_RESPONSE side_update_response);
    public event ITileSideUpdatedEventHandler ITileSideUpdated;

    public delegate void ITileStepChangedEventHandler(STEP_CHANGE_RESPONSE step_change_response);
    public event ITileStepChangedEventHandler ITileStepChanged;

    public delegate void ITileTimedOutEventHandler();
    public event ITileTimedOutEventHandler ITileTimedOut;

    public delegate void PairedITileListReceivedEventHandler(PAIRED_TILES_RESPONSE paired_tile_response);
    public event PairedITileListReceivedEventHandler PairedITileListReceived;

    public delegate void OnlineITileStatusReceivedEventHandler(ONLINE_TILES_RESPONSE online_tile_response);
    public event OnlineITileStatusReceivedEventHandler OnlineITileStatusReceived;

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

    public void OnConnectionStateChanged(int connectionState) 
    {
        ConnectionStateChanged?.Invoke(connectionState);
    }

    public void DiscoveredITilesIDs(string deviceIds) 
    {
        ITilesIDsDiscovered?.Invoke(deviceIds);
    }

    public void ReceiveData(string value) 
    {
        DataReceived?.Invoke(value);
        ReadMessage(value);
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
    private void ReadMessage(string message) 
    {
        ITileMessage iTileMessage = new ITileMessage();
        Debug.Log("ITILE MESSAGE >>>> " + message);
        byte[] byteMessage = HexStringToByteArray(message);

        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]
        iTileMessage.startByte = byteMessage[0];
        iTileMessage.tileId = byteMessage[1];
        iTileMessage.command = byteMessage[2];

        iTileMessage.parameters =  new byte[byteMessage.Length - 3];
        Array.Copy(byteMessage, 3, iTileMessage.parameters, 0, iTileMessage.parameters.Length);

        switch (iTileMessage.command) {
            case (byte)RX_COMMAND.REPLY_PAIRED_TILES:
                PairedITileListReceived?.Invoke(new PAIRED_TILES_RESPONSE(byteMessage));
                break;
            case (byte)RX_COMMAND.REPLY_ONLINE_TILES:
                OnlineITileStatusReceived?.Invoke(new ONLINE_TILES_RESPONSE(byteMessage));
                break;
            case (byte)RX_COMMAND.SHAKE:
                ITileShaked?.Invoke(new SHAKE_RESPONSE(message, byteMessage));
                break;
            case (byte)RX_COMMAND.SIDE_UPDATE:
                ITileSideUpdated?.Invoke(new SIDE_UPDATE_RESPONSE(byteMessage));
                break;
            case (byte)RX_COMMAND.STEP_CHANGE:
                ITileStepChanged?.Invoke(new STEP_CHANGE_RESPONSE(byteMessage));
                break;
            case (byte)RX_COMMAND.TILE_TIMEOUT:
                ITileTimedOut?.Invoke();
                break;
            case (byte)RX_COMMAND.TOUCH:
                ITileTouched?.Invoke(new TOUCH_RESPONSE(message, byteMessage));
                break;
        }
    }

    // Method to send the BROADCAST command with the MASTER tile mac address
    // WORKS!
    public void BroadcastCommand(byte[] masterTileMacAddress)
    {
        SendCommand(TX_COMMAND.BROADCAST, masterTileMacAddress);
    }

    // Method to send the UNPAIR command
    public void UnpairTile(SELECT_ITILE tileID)
    {
        SendCommand(TX_COMMAND.UNPAIR, new byte[] { (byte)tileID });
    }

    // Method to send the QUERY_PAIRED_TILES command
    public void QueryPairedTiles()
    {
        SendCommand(TX_COMMAND.QUERY_PAIRED_TILES, new byte[0]);
    }

    // Method to send the QUERY_ONLINE_TILES command
    public void QueryOnlineTiles()
    {
        SendCommand(TX_COMMAND.QUERY_ONLINE_TILES, new byte[0], SELECT_ITILE.ALL);
    }

    // WORKS!
    public void TriggerLight(
        TILE_COLOR color,
        TIMEOUT_DELAY offAfterSeconds,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    )
    {
        byte[] parameters = new byte[] { color.R, color.G, color.B, (byte)offAfterSeconds, (byte)logReactionTime, (byte)timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_LIGHT, parameters, tileId);
    }

    // Method to play a sound
    // WORKS!
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

    // Method to vibrate a tile
    // WORKS!
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

    // Method to light up tile sides
    // WORKS!
    public void TriggerSide(
        SIDE_COLORS sideColors,
        TIMEOUT_DELAY offAfterSeconds,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    )
    {
        byte[] colors = sideColors.GET_BYTES();
        byte[] parameters = new byte[colors.Length + 3];
        Array.Copy(colors, 0, parameters, 0, colors.Length);
        parameters[^3] = (byte)offAfterSeconds;
        parameters[^2] = (byte)logReactionTime;
        parameters[^1] = (byte)timeoutResponse;
        SendCommand(TX_COMMAND.TRIGGER_SIDE, parameters, tileId);
    }

    public void TriggerEffect() {
        throw new NotImplementedException();
    }

    // Method to trigger tile light, sound, vibration all at once
    // WORKS!
    public void AdvancedTrigger(
        byte redIntensity,
        byte greenIntensity,
        byte blueIntensity,
        TIMEOUT_DELAY timeoutDelay,
        byte soundTrackId,
        byte NOT_IMPLEMENTED_1,
        VIBRATION_PATTERN vibrationPattern,
        REPEAT_COUNT repeatCount,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    ) {
        byte[] parameters = new byte[] { 
            redIntensity, 
            greenIntensity, 
            blueIntensity, 
            (byte)timeoutDelay, 
            soundTrackId,
            NOT_IMPLEMENTED_1,
            (byte)vibrationPattern,
            (byte)repeatCount,
            (byte)logReactionTime,
            (byte)timeoutResponse,
        };
        SendCommand(TX_COMMAND.ADVANCE_TRIGGER, parameters, tileId);
    }

    // WORKS!
    public void TurnOffLights(SELECT_ITILE tileId) 
    {
        SendCommand(TX_COMMAND.OFF_LIGHT, new byte[0], tileId);
    }

    // Method to stop light effect on the tile
    public void StopLightEffect(SELECT_ITILE tileId)
    {
        SendCommand(TX_COMMAND.STOP_EFFECT, new byte[0], tileId);
    }

    // WORKS!
    public void SuperTrigger(
        byte side1RedIntensity, byte side1GreenIntensity, byte side1BlueIntensity,
        byte side2RedIntensity, byte side2GreenIntensity, byte side2BlueIntensity,
        byte side3RedIntensity, byte side3GreenIntensity, byte side3BlueIntensity,
        byte side4RedIntensity, byte side4GreenIntensity, byte side4BlueIntensity,
        byte side5RedIntensity, byte side5GreenIntensity, byte side5BlueIntensity,
        byte side6RedIntensity, byte side6GreenIntensity, byte side6BlueIntensity,
        TIMEOUT_DELAY timeoutDelay,
        byte soundTrackId,
        byte NOT_IMPLEMENTED_1,
        VIBRATION_PATTERN vibrationPattern,
        REPEAT_COUNT repeatCount,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    )
    {
        byte[] parameters = new byte[] {
            side1RedIntensity, side1GreenIntensity, side1BlueIntensity,
            side2RedIntensity, side2GreenIntensity, side2BlueIntensity,
            side3RedIntensity, side3GreenIntensity, side3BlueIntensity,
            side4RedIntensity, side4GreenIntensity, side4BlueIntensity,
            side5RedIntensity, side5GreenIntensity, side5BlueIntensity,
            side6RedIntensity, side6GreenIntensity, side6BlueIntensity,
            (byte)timeoutDelay,
            soundTrackId,
            NOT_IMPLEMENTED_1,
            (byte)vibrationPattern,
            (byte)repeatCount,
            (byte)logReactionTime,
            (byte)timeoutResponse,
        };
        SendCommand(TX_COMMAND.SUPER_TRIGGER, parameters, tileId);
    }

    // Enable or disable accelerometer shake interrupt
    public void ToggleAcceleration(
        TOGGLE_SENSOR toggle,
        SELECT_ITILE tileId
    ) 
    {
        SendCommand(TX_COMMAND.ENABLE_DISABLE_ACCEL, new byte[] { (byte)toggle}, tileId);
    }

    // Set threshold for shake interrupt
    public void SetAccelerationThreshold(
        byte accelerationThreshold,
        SELECT_ITILE tileId
    ) 
    {
        SendCommand(TX_COMMAND.SET_ACCEL_THRESHOLD, new byte[] { accelerationThreshold }, tileId);
    }

    // WORKS!
    public void ToggleTouchSensor(
        TOGGLE_SENSOR toggle,
        SELECT_ITILE tileId
    ) 
    {
        SendCommand(TX_COMMAND.ENABLE_DISABLE_TOUCH, new byte[] { (byte)toggle}, tileId);
    }

    public void SetVolume(byte[] parameters, SELECT_ITILE tileId) 
    {
        throw new NotImplementedException();
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
