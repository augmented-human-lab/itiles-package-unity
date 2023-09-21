namespace ITiles {

    // [APP -> MASTER -> STANDARD] COMMANDS
    // [APP -> MASTER] COMMANDS
    public enum TX_COMMAND: byte {


        START_BYTE = 0x7E,
        END_BYTE = 0xEF,

        // SETUP
        BROADCAST = 0x01,
        ASSIGN_ID = 0x03, // MASTER -> STANDARD
        UNPAIR = 0x04,
        QUERY_PAIRED_TILES = 0x05,
        QUERY_ONLINE_TILES = 0x06,
        
        // IO EVENTS, COMMANDS
        TRIGGER_LIGHT = 0x0B,
        TRIGGER_SOUND = 0x0C,
        TRIGGER_VIBRATE = 0x0D,
        TRIGGER_SIDE = 0x0E,
        TRIGGER_EFFECT = 0x0F,
        ADVANCE_TRIGGER = 0x10,
        SUPER_TRIGGER = 0x16,
        OFF_LIGHT = 0x11,
        STOP_EFFECT = 0x1C,

        // SETTINGS, CONFIGS
        ENABLE_DISABLE_ACCEL = 0x18,
        SET_ACCEL_THRESHOLD = 0x19,
        ENABLE_DISABLE_TOUCH = 0x1A,
        TILE_TIMEOUT = 0x1B

    }

    // [MASTER -> APP] COMMANDS
    // [STANDARD -> MASTER -> APP] COMMANDS
    public enum RX_COMMAND : byte
    {
        START_BYTE = 0xAA,
        REQUEST_TILE_ID = 0x02,
        REPLY_PAIRED_TILES = 0x07,
        REPLY_ONLINE_TILES = 0x08,
        TOUCH = 0x12,
        SIDE_UPDATE = 0x13,
        STEP_CHANGE = 0x14,
        SHAKE = 0x15,
        TILE_TIMEOUT = 0x17,
        END_BYTE = 0xEF
    }

    public class CONFIG_STRINGS {
        public static readonly string ANDROID_UNITY_CALLBACK_INTERFACE = "org.ahlab.itiles.plugin.BLEDataCallback";
        public static readonly string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";
        public static readonly string ANDROID_LIBRARY_MAIN_CLASS = "org.ahlab.itiles.plugin.BLEManager";
        public static readonly string ITILES_BLE_SERVICE_UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
        public static readonly string CHARACTERISTIC_UUID_RX = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
        public static readonly string CHARACTERISTIC_UUID_TX = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
    }

    public class CONNECTION_STATE {
        public static readonly int DISCONNECTED = 0;
        public static readonly int CONNECTING = 1;
        public static readonly int CONNECTED = 2;
        public static readonly int DISCONNECTING = 3;
    }

    public enum SELECT_ITILE: byte {
        MASTER = 0x00,
        I = 0x01,
        II = 0x02,
        III = 0x03,
        IV = 0x04,
        V = 0x05,
        VI = 0x06,
        VII = 0x07,
        ALL = 0xff
    }

    public enum SELECT_SIDE: byte
    {
        I = 0x01,
        II = 0x02,
        III = 0x03,
        IV = 0x04,
        V = 0x05,
        VI = 0x06
    }

    public enum LOG_REACTION_TIME: byte {
        NONE = 0x00,
        TOUCH_OR_STEP = 0x01,
        SHAKE_ONLY = 0x02,
        TOUCH_OR_STEP_OR_SHAKE = 0x03,
        SIDE_PARING = 0x04
    }

    public enum TIMEOUT_DELAY: byte {
        NOPE = 0x00,
        SEC_1 = 0x01,
        SEC_2 = 0x02,
        SEC_3 = 0x05,
        SEC_4 = 0X04,
        SEC_5 = 0x05,
        SEC_10 = 0x0a,
        SEC_15 = 0x0f,
        SEC_20 = 0x14,
        SEC_30 = 0x1e,
        SEC_40 = 0x28,
        SEC_45 = 0x2d,
        SEC_90 = 0x5a,
        MIN_1 = 0x3c,
        MIN_2 = 0x78,
        MIN_3 = 0xb4,
        MIN_4 = 0xf0,
        MIN_4_25 = 0xff
    }

    public enum TIMEOUT_RESPONSE: byte
    {
        NOPE = 0x00,
        SEC_1 = 0x01,
        SEC_2 = 0x02,
        SEC_3 = 0x05,
        SEC_4 = 0X04,
        SEC_5 = 0x05,
        SEC_10 = 0x0a,
        SEC_15 = 0x0f,
        SEC_20 = 0x14,
        SEC_30 = 0x1e,
        SEC_40 = 0x28,
        SEC_45 = 0x2d,
        SEC_90 = 0x5a,
        MIN_1 = 0x3c,
        MIN_2 = 0x78,
        MIN_3 = 0xb4,
        MIN_4 = 0xf0,
        MIN_4_25 = 0xff
    }

    public enum STEP: byte {
        ON_ITILE = 0x00,
        OFF_ITILE = 0x01
    }

    public enum SIDE: byte
    {
        UNPAIR = 0x00,
        PAIR = 0x01
    }

    public enum VIBRATION_PATTERN: byte {
        NONE = 0x00,
        I = 0x01,
        II = 0x02,
        III = 0x03,
        IV = 0x04,
        V = 0x05,
        VI = 0x06,
        VII = 0x07,
        VIII = 0x08,
        IX = 0x09
    }

    public enum REPEAT_COUNT : byte {
        I = 0x01,
        II = 0x02,
        III = 0x03,
        IV = 0x04,
        V = 0x05,
        VI = 0x06,
        VII = 0x07,
        VIII = 0x08,
        IX = 0x09
    }

    public class SHAKE_DETECTOR {
        public static readonly byte OFF = 0x00;
        public static readonly byte ON = 0x01;
    }

    public class TOUCH_SENSOR {
        public static readonly byte OFF = 0x00;
        public static readonly byte ON = 0x01;
    }

    public class ANDROID_ITILE_METHOD {
        public static readonly string CONNECT = "connect";
        public static readonly string START_SCAN = "startSearchingITiles";
        public static readonly string STOP_SCAN = "stopSearchingITiles";
        public static readonly string SET_CALLBACK = "setDataCallback";
        public static readonly string START_READ = "startReadingDataStream";
        public static readonly string WRITE = "write";
    }

    public class EDGE_COLOR {
        public static readonly byte[] PRESET_1 = new byte[] {
            0x00, 0x00, 0x99, // side 1
            0x00, 0x99, 0x00, // side 2
            0x99, 0x00, 0x00, // side 3
            0x99, 0x99, 0x00, // side 4
            0x00, 0x99, 0x99, // side 5
            0x99, 0x00, 0x99  // side 6       
        };
    }

    public class TILE_COLOR {
        public static readonly byte[] WHITE = new byte[] {
            0x99, 0x99, 0x99,
        };
        public static readonly byte[] RED = new byte[] {
            0x99, 0x00, 0x00,
        };
        public static readonly byte[] GREEN = new byte[] {
            0x00, 0x99, 0x00,
        };
        public static readonly byte[] BLUE = new byte[] {
            0x00, 0x00, 0x99,
        };
        public static readonly byte[] CYAN = new byte[] {
            0x00, 0x99, 0x99,
        };
        public static readonly byte[] YELLOW = new byte[] {
            0x99, 0x99, 0x00,
        };
        public static readonly byte[] MAGENTA = new byte[] {
            0x99, 0x00, 0x99,
        };
    }
}
