using UnityEngine;
using System;
using ITiles;
using System.Collections.Generic;

public class BLEController : MonoBehaviour
{
    private AndroidJavaObject bleManager;

    #region Subscribable itile events

    public delegate void DataReceivedEventHandler(string data);
    public event DataReceivedEventHandler DataReceived;

    public delegate void MasterTilesDiscoveredEventHandler(List<string> discovered_itiles);
    public event MasterTilesDiscoveredEventHandler MasterTilesDiscovered;

    public delegate void ConnectionStateChangedEventHandler(CONNECTION_STATE connectionState);
    public event ConnectionStateChangedEventHandler ConnectionStateChanged;

    public delegate void ITileTouchedEventHandler(TOUCH_RESPONSE touch_response);
    public event ITileTouchedEventHandler ITileTouched;

    public delegate void ITileShakedEventHandler(SHAKE_RESPONSE shake_response);
    public event ITileShakedEventHandler ITileShaked;

    public delegate void ITileSidePairedEventHandler(SIDE_PAIR_RESPONSE side_pair_response);
    public event ITileSidePairedEventHandler ITileSidePaired;

    public delegate void ITileStepChangedEventHandler(STEP_CHANGE_RESPONSE step_change_response);
    public event ITileStepChangedEventHandler ITileStepChanged;

    public delegate void ITileTimedOutEventHandler();
    public event ITileTimedOutEventHandler ITileTimedOut;

    public delegate void PairedITileListReceivedEventHandler(PAIRED_TILES_RESPONSE paired_tile_response);
    public event PairedITileListReceivedEventHandler PairedITileListReceived;

    public delegate void OnlineITileStatusReceivedEventHandler(ONLINE_TILES_RESPONSE online_tile_response);
    public event OnlineITileStatusReceivedEventHandler OnlineITileStatusReceived;

    public delegate void BattaryStatusReceivedEventHandler(BATTARY_STATUS_RESPONSE battary_status_response);
    public event BattaryStatusReceivedEventHandler BattaryStatusReceived;

    #endregion

    public static BLEController itile;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        itile = this;
    }

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

    public void OnConnectionStateChanged(CONNECTION_STATE connectionState) 
    {
        ConnectionStateChanged?.Invoke(connectionState);
    }

    public void DiscoveredMasterTiles(string deviceIds) 
    {
        string preProcessedJson = "{\"discoveredItileIds\": #}".Replace("#", deviceIds);
        TileIdList jsonData = JsonUtility.FromJson<TileIdList>(preProcessedJson);
        List<string> masterTileIds = new List<string>();
        foreach (string id in jsonData.discoveredItileIds)
        {
            masterTileIds.Add(id);
        }
        MasterTilesDiscovered?.Invoke(masterTileIds);
    }

    public void ReceiveData(string value) 
    {
        DataReceived?.Invoke(value);
        DecodeMessage(value);
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

    private void DecodeMessage(string message) 
    {
        byte[] byteMessage = HexStringToByteArray(message);

        // Command packet format: [Start Byte][Tile ID][Command][Length][Parameters][End Byte]

        switch ((RX_COMMAND)byteMessage[2]) {
            case RX_COMMAND.REPLY_PAIRED_TILES:
                PairedITileListReceived?.Invoke(new PAIRED_TILES_RESPONSE(byteMessage));
                break;
            case RX_COMMAND.REPLY_ONLINE_TILES:
                OnlineITileStatusReceived?.Invoke(new ONLINE_TILES_RESPONSE(byteMessage));
                break;
            case RX_COMMAND.SHAKE:
                ITileShaked?.Invoke(new SHAKE_RESPONSE(byteMessage));
                break;
            case RX_COMMAND.SIDE_UPDATE:
                ITileSidePaired?.Invoke(new SIDE_PAIR_RESPONSE(byteMessage));
                break;
            case RX_COMMAND.STEP_CHANGE:
                ITileStepChanged?.Invoke(new STEP_CHANGE_RESPONSE(byteMessage));
                break;
            case RX_COMMAND.TILE_TIMEOUT:
                ITileTimedOut?.Invoke();
                break;
            case RX_COMMAND.TOUCH:
                ITileTouched?.Invoke(new TOUCH_RESPONSE(byteMessage));
                break;
            case RX_COMMAND.REPLY_BATTERY_LEVEL:
                BattaryStatusReceived?.Invoke(new BATTARY_STATUS_RESPONSE(byteMessage));
                break;
                
        }
    }

    public void PairTiles(byte[] masterTileMacAddress)
    {
        SendCommand(TX_COMMAND.BROADCAST, masterTileMacAddress);
    }

    public void UnpairTile(SELECT_ITILE tileID)
    {
        SendCommand(TX_COMMAND.UNPAIR, new byte[0], tileID);
    }

    public void QueryPairedTiles()
    {
        SendCommand(TX_COMMAND.QUERY_PAIRED_TILES, new byte[0]);
    }

    public void QueryOnlineTiles()
    {
        SendCommand(TX_COMMAND.QUERY_ONLINE_TILES, new byte[0], SELECT_ITILE.ALL);
    }

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

    public void TriggerSound(
        SOUND_TRACK soundTrackID,
        REPEAT_COUNT repeatCount,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    )
    {
        byte[] parameters = new byte[] { (byte)soundTrackID, (byte)repeatCount, (byte)logReactionTime, (byte)timeoutResponse };
        SendCommand(TX_COMMAND.TRIGGER_SOUND, parameters, tileId);
    }

    public void TriggerVibration(
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
    public void TriggerLight(
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

    public void TriggerLightSoundVibration(
        TILE_COLOR lightColor,
        TIMEOUT_DELAY timeoutDelay,
        SOUND_TRACK soundTrackId,
        VIBRATION_PATTERN vibrationPattern,
        REPEAT_COUNT repeatCount,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    ) {
        byte[] parameters = new byte[] { 
            lightColor.R, 
            lightColor.G, 
            lightColor.B, 
            (byte)timeoutDelay, 
            (byte)soundTrackId,
            0x00, // not implemented
            (byte)vibrationPattern,
            (byte)repeatCount,
            (byte)logReactionTime,
            (byte)timeoutResponse,
        };
        SendCommand(TX_COMMAND.ADVANCE_TRIGGER, parameters, tileId);
    }

    public void TurnOffLight(SELECT_ITILE tileId) 
    {
        SendCommand(TX_COMMAND.OFF_LIGHT, new byte[0], tileId);
    }

    public void StopEffect(SELECT_ITILE tileId)
    {
        SendCommand(TX_COMMAND.STOP_EFFECT, new byte[0], tileId);
    }

    public void TriggerLightSoundVibration(
        SIDE_COLORS sideColor,
        TIMEOUT_DELAY timeoutDelay,
        SOUND_TRACK soundTrackId,
        VIBRATION_PATTERN vibrationPattern,
        REPEAT_COUNT repeatCount,
        LOG_REACTION_TIME logReactionTime,
        TIMEOUT_RESPONSE timeoutResponse,
        SELECT_ITILE tileId
    )
    {
        byte[] parameters = new byte[] {
            sideColor.SIDE_1.R, sideColor.SIDE_1.G, sideColor.SIDE_1.B,
            sideColor.SIDE_2.R, sideColor.SIDE_2.G, sideColor.SIDE_2.B,
            sideColor.SIDE_3.R, sideColor.SIDE_3.G, sideColor.SIDE_3.B,
            sideColor.SIDE_4.R, sideColor.SIDE_4.G, sideColor.SIDE_4.B,
            sideColor.SIDE_5.R, sideColor.SIDE_5.G, sideColor.SIDE_5.B,
            sideColor.SIDE_6.R, sideColor.SIDE_6.G, sideColor.SIDE_6.B,
            (byte)timeoutDelay,
            (byte)soundTrackId,
            0x00, // not implemented
            (byte)vibrationPattern,
            (byte)repeatCount,
            (byte)logReactionTime,
            (byte)timeoutResponse,
        };
        SendCommand(TX_COMMAND.SUPER_TRIGGER, parameters, tileId);
    }

    public void ToggleShakeSensor(
        TOGGLE_SENSOR toggle,
        SELECT_ITILE tileId
    ) 
    {
        SendCommand(TX_COMMAND.ENABLE_DISABLE_ACCEL, new byte[] { (byte)toggle}, tileId);
    }

    public void SetShakeThreshold(
        byte accelerationThreshold,
        SELECT_ITILE tileId
    ) 
    {
        SendCommand(TX_COMMAND.SET_ACCEL_THRESHOLD, new byte[] { accelerationThreshold }, tileId);
    }

    public void ToggleTouchSensor(
        TOGGLE_SENSOR toggle,
        SELECT_ITILE tileId
    ) 
    {
        SendCommand(TX_COMMAND.ENABLE_DISABLE_TOUCH, new byte[] { (byte)toggle}, tileId);
    }

    public void ConfirmAssignement(
        SELECT_ITILE tileId
    )
    {
        SendCommand(TX_COMMAND.CONFIRM_ASSIGNMENT, new byte[0], tileId);
    }

    public void GameInProgress(
        GAME_STATUS gameStatus,
        SELECT_ITILE tileId
    )
    {
        SendCommand(TX_COMMAND.GAME_IN_PROGRESS, new byte[] {(byte)gameStatus }, tileId);
    }

    public void AssignFeedback(
        FEEDBACK_STATUS feedbackStatus,
        TILE_COLOR color,
        SOUND_TRACK soundTrackId,
        VIBRATION_PATTERN vibrationPattern,
        TIMEOUT_DELAY timeoutDelay,
        SELECT_ITILE tileId
    ) {
        SendCommand(TX_COMMAND.ASSIGN_FEEDBACK, new byte[] {(byte)feedbackStatus, (byte)color.R, color.G, color.B, (byte)soundTrackId, (byte)vibrationPattern, (byte)timeoutDelay }, tileId);
    }

    public void GetBattery(
        SELECT_ITILE tileId    
    ) {
        SendCommand(TX_COMMAND.GET_BATTERY_LEVEL, new byte[0], tileId);
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

    #endregion

}

[System.Serializable]
public class TileIdList
{
    public string[] discoveredItileIds;
}
