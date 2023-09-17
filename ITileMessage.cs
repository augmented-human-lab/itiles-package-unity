[System.Serializable]
public class ITileMessage
{
    public byte startByte;
    public byte tileId;
    public byte command;
    public byte length;
    public byte[] parameters;
    public byte endByte;
}
