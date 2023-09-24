# itiles-package-unity
Enables communication between unity and iTiles hardware units

# Installation
- Open unity package manger
- Press `+` button then `Add package from git url`
- Copy this git repo url there.

# Setup
- Drag & drop `BLEController` prefab from `ITiles/Prefabs`

# Use
All Methods are internally TX commands sent to itile server (master tile).
All Events are internally RX messages received from itile server (master tile).
Master tile is responsible for communicating with paired standard tiles.
Method call example `bleController_instance.Method(params)`
Subscribe to events from Unity `Start` method `bleController_instance.Event += EventHandler`
Unsubscribe events from Unity `OnDestroy` method `bleController_instance.Event -= EventHandler`

## Import ITiles
```csharp
using ITiles;
```

## Methods (TX Messages to iTile Server)
```csharp
void Connect();
```

```csharp
void UnpairTile(SELECT_ITILE tileID)
```

```csharp
void BroadcastCommand(byte[] masterTileMacAddress)
```

```csharp
void QueryPairedTiles()
```

```csharp
void QueryOnlineTiles()
```

```csharp
TriggerLight (
  TILE_COLOR color, 
  TIMEOUT_DELAY offAfterSeconds, 
  LOG_REACTION_TIME logReactionTime, 
  TIMEOUT_RESPONSE timeoutResponse, 
  SELECT_ITILE tileId
)
```

```csharp
void TriggerSound (
  byte soundTrackID,
  REPEAT_COUNT repeatCount,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void TriggerVibrate(
  VIBRATION_PATTERN vibrationPatternID,
  REPEAT_COUNT repeatCount,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void TriggerSide(
  SIDE_COLORS sideColors,
  TIMEOUT_DELAY offAfterSeconds,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void AdvancedTrigger(
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
)
```

```csharp
void TurnOffLights(SELECT_ITILE tileId)
```

```csharp
void SuperTrigger(
  SIDE_COLORS sideColor,
  TIMEOUT_DELAY timeoutDelay,
  byte soundTrackId,
  byte NOT_IMPLEMENTED_1,
  VIBRATION_PATTERN vibrationPattern,
  REPEAT_COUNT repeatCount,
  LOG_REACTION_TIME logReactionTime,
  TIMEOUT_RESPONSE timeoutResponse,
  SELECT_ITILE tileId
)
```

```csharp
void ToggleTouchSensor(
  TOGGLE_SENSOR toggle,
  SELECT_ITILE tileId
) 
```

```csharp
void ToggleAcceleration(
  TOGGLE_SENSOR toggle,
  SELECT_ITILE tileId
) 
```

```csharp
void SetAccelerationThreshold(
  byte accelerationThreshold,
  SELECT_ITILE tileId
) 
```

## Events (RX Messages from iTile Server)
- `DataReceived(string data)` raw message received from a itile
- `ITilesIDsDiscovered(List<string> discovered_itiles)` mac addresses of any visible iTile server (master tile)
- `ConnectionStateChanged(CONNECTION_STATE connectionState)` notifies the status of ble connection with iTile server (master tile)

## Events - Interactions
- `ITileTouched(TOUCH_RESPONSE touchResponse)` notifies a itile has been touched
- `ITileShaked(SHAKE_RESPONSE shakeResponse)` notifies a itile has been shaked
- `ITileStepChanged(STEP_CHANGE_RESPONSE stepChangeResponse)` notifies someone has step on or off a itile
- `ITileTimedOut` notifies a itile has been timed out
- `ITileSideUpdated(SIDE_UPDATE_RESPONSE sideUpdateResponse)` notifies a itile side is paired with another
- `PairedITileListReceived(PAIRED_TILES_RESPONSE pairedTilesResponse)` requested paired itile list is received
- `OnlineITileStatusReceived(ONLINE_TILES_RESPONSE onlineTilesResponse)` requested online itile details are received from each tile
