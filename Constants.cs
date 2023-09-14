namespace ITiles {
   
    public class TX_COMMAND {
        // [APP -> MASTER -> STANDARD] COMMANDS
        // [APP -> MASTER] COMMANDS
        public static readonly byte BROADCAST = 0x01;
        public static readonly byte REQUEST_TILE_ID = 0x02; // STANDARD -> MASTER
        public static readonly byte ASSIGN_ID = 0x03; // MASTER -> STANDARD
        public static readonly byte UNPAIR = 0x04;
        public static readonly byte QUERY_PAIRED_TILES = 0x05;
        public static readonly byte QUERY_ONLINE_TILES = 0x06;
        
        public static readonly byte TRIGGER_LIGHT = 0x0B;
        public static readonly byte TRIGGER_SOUND = 0x0C;
        public static readonly byte TRIGGER_VIBRATE = 0x0D;
        public static readonly byte TRIGGER_SIDE = 0x0E;
        public static readonly byte TRIGGER_EFFECT = 0x0F;
        public static readonly byte ADVANCE_TRIGGER = 0x10;
        public static readonly byte OFF_LIGHT = 0x11;

        public static readonly byte SUPER_TRIGGER = 0x16;
        public static readonly byte ENABLE_DISABLE_ACCEL = 0x18;
        public static readonly byte SET_ACCEL_THRESHOLD = 0x19;
        public static readonly byte ENABLE_DISABLE_TOUCH = 0x1A;
        public static readonly byte TILE_TIMEOUT = 0x1B;
        public static readonly byte STOP_EFFECT = 0x1C;

    }

    public class RX_COMMAND {
        // [MASTER -> APP] COMMANDS
        // [STANDARD -> MASTER -> APP] COMMANDS
        public static readonly byte REPLY_PAIRED_TILES = 0x07;
        public static readonly byte REPLY_ONLINE_TILES = 0x08;
        public static readonly byte TOUCH = 0x12;
        public static readonly byte SIDE_UPDATE = 0x13;
        public static readonly byte STEOP_CHANGE = 0x14;
        public static readonly byte SHAKE = 0x15;
        public static readonly byte TILE_TIMEOUT = 0x02;

    }

    public class CONFIG_STRINGS {
        public static readonly string ANDROID_UNITY_CALLBACK_INTERFACE = "org.ahlab.itiles.plugin.BLEDataCallback";
        public static readonly string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";
        public static readonly string ANDROID_LIBRARY_MAIN_CLASS = "org.ahlab.itiles.plugin.BLEManager";
        public static readonly string ITILES_BLE_SERVICE_UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
        public static readonly string CHARACTERISTIC_UUID_RX = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
        public static readonly string CHARACTERISTIC_UUID_TX = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
    }
}
