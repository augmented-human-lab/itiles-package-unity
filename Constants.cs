namespace ITiles {

    // [APP -> MASTER -> STANDARD] COMMANDS
    // [APP -> MASTER] COMMANDS
    public enum TX_COMMAND: byte {


        START_BYTE = 0x7E,
        END_BYTE = 0xEF,

        // SETUP
        BROADCAST = 0x01,
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

    public enum CONNECTION_STATE: int {
        DISCONNECTED = 0,
        CONNECTING = 1,
        CONNECTED = 2,
        DISCONNECTING = 3
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
        IMMEDIATE = 0x00,
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

    public enum TOGGLE_SENSOR: byte {
        OFF = 0x00,
        ON = 0x01
    }

    public class ANDROID_ITILE_METHOD {
        public static readonly string CONNECT = "connect";
        public static readonly string START_SCAN = "startSearchingITiles";
        public static readonly string STOP_SCAN = "stopSearchingITiles";
        public static readonly string SET_CALLBACK = "setDataCallback";
        public static readonly string START_READ = "startReadingDataStream";
        public static readonly string WRITE = "write";
    }

    public struct SIDE_COLORS {
        public TILE_COLOR SIDE_1;
        public TILE_COLOR SIDE_2;
        public TILE_COLOR SIDE_3;
        public TILE_COLOR SIDE_4;
        public TILE_COLOR SIDE_5;
        public TILE_COLOR SIDE_6;
        public SIDE_COLORS(byte[] colors) {
            SIDE_1 = new TILE_COLOR(colors[0], colors[1], colors[2]);
            SIDE_2 = new TILE_COLOR(colors[3], colors[4], colors[5]);
            SIDE_3 = new TILE_COLOR(colors[6], colors[7], colors[8]);
            SIDE_4 = new TILE_COLOR(colors[9], colors[10], colors[11]);
            SIDE_5 = new TILE_COLOR(colors[12], colors[11], colors[12]);
            SIDE_6 = new TILE_COLOR(colors[15], colors[16], colors[17]);
        }
        public SIDE_COLORS(TILE_COLOR side1, TILE_COLOR side2, TILE_COLOR side3, TILE_COLOR side4, TILE_COLOR side5, TILE_COLOR side6) {
            SIDE_1 = side1;
            SIDE_2 = side2;
            SIDE_3 = side3;
            SIDE_4 = side4;
            SIDE_5 = side5;
            SIDE_6 = side6;
        }
        public SIDE_COLORS(
            byte side_1_r, byte side_1_g, byte side_1_b,
            byte side_2_r, byte side_2_g, byte side_2_b,
            byte side_3_r, byte side_3_g, byte side_3_b,
            byte side_4_r, byte side_4_g, byte side_4_b,
            byte side_5_r, byte side_5_g, byte side_5_b,
            byte side_6_r, byte side_6_g, byte side_6_b
        ) {
            SIDE_1 = new TILE_COLOR(side_1_r, side_1_g, side_1_b);
            SIDE_2 = new TILE_COLOR(side_2_r, side_2_g, side_2_b);
            SIDE_3 = new TILE_COLOR(side_3_r, side_3_g, side_3_b);
            SIDE_4 = new TILE_COLOR(side_4_r, side_4_g, side_4_b);
            SIDE_5 = new TILE_COLOR(side_5_r, side_5_g, side_5_b);
            SIDE_6 = new TILE_COLOR(side_6_r, side_6_g, side_6_b);
        }
        public byte[] GET_BYTES() {
            return new byte[18] {
                SIDE_1.R, SIDE_1.G, SIDE_1.B,
                SIDE_2.R, SIDE_2.G, SIDE_2.B,
                SIDE_3.R, SIDE_3.G, SIDE_3.B,
                SIDE_4.R, SIDE_4.G, SIDE_4.B,
                SIDE_5.R, SIDE_5.G, SIDE_5.B,
                SIDE_6.R, SIDE_6.G, SIDE_6.B
            };
        }
        public static SIDE_COLORS RANDOM() {
            byte[] randomColorSet = new byte[18];
            System.Random random = new System.Random();
            for (int i = 0; i < randomColorSet.Length; i++)
            {
                randomColorSet[i] = (byte)random.Next(100);
            }
            return new SIDE_COLORS(randomColorSet);
        }
    }

    public struct TILE_COLOR {
        public byte R;
        public byte G;
        public byte B;
        public TILE_COLOR(byte[] rgb) {
            R = rgb[0];
            G = rgb[1];
            B = rgb[2];
        }
        public TILE_COLOR(byte r, byte g, byte b) {
            R = r;
            G = g;
            B = b;
        }
        public byte[] GET_BYTES()
        {
            return new byte[3] {
                R, G, B
            };
        }
        public static TILE_COLOR RANDOM()
        {
            byte[] randomColor = new byte[3];
            System.Random random = new System.Random();
            for (int i = 0; i < randomColor.Length; i++)
            {
                randomColor[i] = (byte)random.Next(100);
            }
            return new TILE_COLOR(randomColor);
        }
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

    public struct TOUCH_RESPONSE {
        public string string_message;
        public byte[] byte_message;
        public byte tile_id;
        public byte reaction_time_high_byte;
        public byte reaction_time_low_byte;
        public TOUCH_RESPONSE(string string_message, byte[] byte_message, byte tile_id, byte reaction_time_low_byte, byte reaction_time_high_byte)
        {
            this.string_message = string_message;
            this.byte_message = byte_message;
            this.tile_id = tile_id;
            this.reaction_time_high_byte = reaction_time_high_byte;
            this.reaction_time_low_byte = reaction_time_low_byte;
        }
        public int GetReactionTime() {
            return (reaction_time_high_byte << 8) | reaction_time_low_byte;
        }
    }

}
