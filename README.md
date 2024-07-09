# itiles-package-unity
Enables communication between unity and iTiles hardware units

# Installation
- Open unity package manger
- Press `+` button then `Add package from git url`
- Copy this git repo url there.

# Setup
- Drag & drop `BLEController` prefab from `ITiles/Prefabs`

# Use
- All Methods are internally TX commands sent to itile server (master tile).
- All Events are internally RX messages received from itile server (master tile).
- Master tile is responsible for communicating with paired standard tiles.
- Method call example `bleController_instance.Method(params)`
- Subscribe to events from Unity `Start` method `bleController_instance.Event += EventHandler`
- Unsubscribe events from Unity `OnDestroy` method `bleController_instance.Event -= EventHandler`

## Import ITiles
```csharp
using ITiles;
```

## Methods (TX Messages to iTile Server)
```csharp
// to initiate itile server scan
// Subscribe to `MasterTilesDiscovered` event to receive discovered itile server ids (mac addresses)
void StartScan()
```

```csharp
// to cancel itile server scan. 
// This method will be internlly called after connecting to a itile server
void StopScan()
```

```csharp
// to connect with already discovered itile server
void Connect(string deviceAddress);
```

```csharp
// to initiate standard tile paring
// once initiated touch on standard tiles to complete pair with master
void PairTiles(byte[] masterTileMacAddress)
```

```csharp
// initiate unparing one or all standard tiles from master
void UnpairTile(SELECT_ITILE tileID)
```

```csharp
// to clear the array of tile mac address and deregister tiles from ESPNOW
void ClearMacList()
```

```csharp
// to receive list of paired tile Ids 
// Subscribe to `PairedITileListReceived` event to receive paired itile id list
void QueryPairedTiles()
```

```csharp
// to command each online tile to reply with it's status including battary percentage
// Subscribe to `OnlineITileStatusReceived` event to receive status of given tile (default value is ALL)
void QueryOnlineTiles(SELECT_ITILE tileID = SELECT_ITILE.ALL)
```

```csharp
void TriggerLight (
  TILE_COLOR color | SIDE_COLORS sideColors, 
  TIMEOUT_DELAY offAfterSeconds, 
  LOG_REACTION_TIME logReactionTime, 
  TIMEOUT_RESPONSE timeoutResponse, 
  SELECT_ITILE tileId
)
```

```csharp
void TriggerSound (
  SOUND_TRACK soundTrackID,
  REPEAT_COUNT repeatCount,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void TriggerVibration(
  VIBRATION_PATTERN vibrationPatternID,
  REPEAT_COUNT repeatCount,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void TriggerLightSoundVibration(
  TILE_COLOR tileColor | SIDE_COLORS sideColor,
  TIMEOUT_DELAY timeoutDelay,
  SOUND_TRACK soundTrackId,
  VIBRATION_PATTERN vibrationPattern,
  REPEAT_COUNT repeatCount,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void TurnOffLight(SELECT_ITILE tileId)
```

```csharp
void ToggleTouchSensor(TOGGLE_SENSOR toggle, SELECT_ITILE tileId) 
```

```csharp
void ToggleShakeSensor(TOGGLE_SENSOR toggle, SELECT_ITILE tileId) 
```

```csharp
void SetShakeThreshold(byte accelerationThreshold, SELECT_ITILE tileId) 
```

```csharp
void ConfirmAssignement(SELECT_ITILE tileId) 
```

```csharp
void GameInProgress(GAME_STATUS gameStatus, SELECT_ITILE tileId)
```

```csharp
void AssignFeedback(FEEDBACK_STATUS feedbackStatus, TILE_COLOR color, SOUND_TRACK soundTrackId, VIBRATION_PATTERN vibrationPattern, TIMEOUT_DELAY timeoutDelay, SELECT_ITILE tileId)
```

```csharp
void GetBattery(SELECT_ITILE tileId)
```


## Events - Basic (RX Messages from iTile Server)

```csharp
// raw message received from a itile
DataReceived(string data)
```

```csharp
// mac addresses of any visible iTile server (master tile)
MasterTilesDiscovered(List<string> discovered_itiles)
```

```csharp
// notifies the status of ble connection with iTile server (master tile)
ConnectionStateChanged(CONNECTION_STATE connectionState)
```

```csharp
// requested paired itile list is received
PairedITileListReceived(PAIRED_TILES_RESPONSE pairedTilesResponse)
```

```csharp
// requested online itile details are received from each tile
OnlineITileStatusReceived(ONLINE_TILES_RESPONSE onlineTilesResponse)
```

```csharp
// requested battary status is received from specified tile
BattaryStatusReceived(ONLINE_TILES_RESPONSE onlineTilesResponse)
```

## Events - Interactions (RX Messages from iTile Server)

```csharp
// notifies a itile has been touched
ITileTouched(TOUCH_RESPONSE touchResponse)
```

```csharp
// notifies a itile has been shaked
ITileShaked(SHAKE_RESPONSE shakeResponse)
```

```csharp
// notifies someone has step on or off a itile
ITileStepChanged(STEP_CHANGE_RESPONSE stepChangeResponse)
```

```csharp
// notifies a itile has been timed out
ITileTimedOut()
```

```csharp
// notifies a itile side is paired with another
ITileSidePaired(SIDE_PAIR_RESPONSE sidePairResponse)
```



