namespace ITiles {

    // [APP -> MASTER -> STANDARD] COMMANDS
    // [APP -> MASTER] COMMANDS
    public class TX_COMMAND {


        public static readonly byte START_BYTE = 0x7E;
        public static readonly byte END_BYTE = 0xEF;

        // SETUP
        public static readonly byte BROADCAST = 0x01;
        public static readonly byte REQUEST_TILE_ID = 0x02; // STANDARD -> MASTER
        public static readonly byte ASSIGN_ID = 0x03; // MASTER -> STANDARD
        public static readonly byte UNPAIR = 0x04;
        public static readonly byte QUERY_PAIRED_TILES = 0x05;
        public static readonly byte QUERY_ONLINE_TILES = 0x06;
        
        // IO EVENTS, COMMANDS
        public static readonly byte TRIGGER_LIGHT = 0x0B;
        public static readonly byte TRIGGER_SOUND = 0x0C;
        public static readonly byte TRIGGER_VIBRATE = 0x0D;
        public static readonly byte TRIGGER_SIDE = 0x0E;
        public static readonly byte TRIGGER_EFFECT = 0x0F;
        public static readonly byte ADVANCE_TRIGGER = 0x10;
        public static readonly byte SUPER_TRIGGER = 0x16;
        public static readonly byte OFF_LIGHT = 0x11;
        public static readonly byte STOP_EFFECT = 0x1C;

        // SETTINGS, CONFIGS
        public static readonly byte ENABLE_DISABLE_ACCEL = 0x18;
        public static readonly byte SET_ACCEL_THRESHOLD = 0x19;
        public static readonly byte ENABLE_DISABLE_TOUCH = 0x1A;
        public static readonly byte TILE_TIMEOUT = 0x1B;

    }

    // [MASTER -> APP] COMMANDS
    // [STANDARD -> MASTER -> APP] COMMANDS
    public enum RX_COMMAND : byte
    {
        START_BYTE = 0xAA,
        REPLY_PAIRED_TILES = 0x07,
        REPLY_ONLINE_TILES = 0x08,
        TOUCH = 0x12,
        SIDE_UPDATE = 0x13,
        STEP_CHANGE = 0x14,
        SHAKE = 0x15,
        TILE_TIMEOUT = 0x02,
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

    public class SELECT_ITILE {
        public static readonly byte MASTER = 0x00;
        public static readonly byte I = 0x01;
        public static readonly byte II = 0x01;
        public static readonly byte III = 0x01;
        public static readonly byte IV = 0x01;
        public static readonly byte V = 0x01;
        public static readonly byte VI = 0x01;
        public static readonly byte VII = 0x01;
        public static readonly byte ALL = 0xff;
    }

    public class SELECT_SIDE
    {
        public static readonly byte I = 0x01;
        public static readonly byte II = 0x01;
        public static readonly byte III = 0x01;
        public static readonly byte IV = 0x01;
        public static readonly byte V = 0x01;
        public static readonly byte VI = 0x01;
    }

    public class LOG_REACTION_TIME {
        public static readonly byte NONE = 0x00;
        public static readonly byte TOUCH_OR_STEP = 0x01;
        public static readonly byte SHAKE_ONLY = 0x02;
        public static readonly byte TOUCH_OR_STEP_OR_SHAKE = 0x03;
        public static readonly byte SIDE_PARING = 0x04;
    }

    public class TIMEOUT_DELAY {
        public static readonly byte NOPE = 0x00;
        public static readonly byte SEC_1 = 0x01;
        public static readonly byte SEC_2 = 0x02;
        public static readonly byte SEC_3 = 0x05;
        public static readonly byte SEC_4 = 0X04;
        public static readonly byte SEC_5 = 0x05;
        public static readonly byte SEC_10 = 0x0a;
        public static readonly byte SEC_15 = 0x0f;
        public static readonly byte SEC_20 = 0x14;
        public static readonly byte SEC_30 = 0x1e;
        public static readonly byte SEC_40 = 0x28;
        public static readonly byte SEC_45 = 0x2d;
        public static readonly byte SEC_90 = 0x5a;
        public static readonly byte MIN_1 = 0x3c;
        public static readonly byte MIN_2 = 0x78;
        public static readonly byte MIN_3 = 0xb4;
        public static readonly byte MIN_4 = 0xf0;
        public static readonly byte MIN_4_25 = 0xff;
    }

    public class TIMEOUT_RESPONSE
    {
        public static readonly byte NOPE = 0x00;
        public static readonly byte SEC_1 = 0x01;
        public static readonly byte SEC_2 = 0x02;
        public static readonly byte SEC_3 = 0x05;
        public static readonly byte SEC_4 = 0X04;
        public static readonly byte SEC_5 = 0x05;
        public static readonly byte SEC_10 = 0x0a;
        public static readonly byte SEC_15 = 0x0f;
        public static readonly byte SEC_20 = 0x14;
        public static readonly byte SEC_30 = 0x1e;
        public static readonly byte SEC_40 = 0x28;
        public static readonly byte SEC_45 = 0x2d;
        public static readonly byte SEC_90 = 0x5a;
        public static readonly byte MIN_1 = 0x3c;
        public static readonly byte MIN_2 = 0x78;
        public static readonly byte MIN_3 = 0xb4;
        public static readonly byte MIN_4 = 0xf0;
        public static readonly byte MIN_4_25 = 0xff;
    }

    public class STEP {
        public static readonly byte ON_ITILE = 0x00;
        public static readonly byte OFF_ITILE = 0x01;
    }

    public class SIDE
    {
        public static readonly byte UNPAIR = 0x00;
        public static readonly byte PAIR = 0x01;
    }

    public class VIBRATION_PATTERN {
        public static readonly byte NONE = 0x00;
        public static readonly byte I = 0x01;
        public static readonly byte II = 0x02;
        public static readonly byte III = 0x03;
        public static readonly byte IV = 0x04;
        public static readonly byte V = 0x05;
        public static readonly byte VI = 0x06;
        public static readonly byte VII = 0x07;
        public static readonly byte VIII = 0x08;
        public static readonly byte IX = 0x09;
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
