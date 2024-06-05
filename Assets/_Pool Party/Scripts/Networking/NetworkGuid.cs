using Unity.Netcode;

public struct NetworkGuid : INetworkSerializable
{
    public ulong FirstHalf;
    public ulong SecondHalf;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref FirstHalf);
        serializer.SerializeValue(ref SecondHalf);
    }
}

public static class NetworkGuidExtensions
{
    public static NetworkGuid ToNetworkGuid(this System.Guid id)
    {
        var networkId = new NetworkGuid();
        networkId.FirstHalf = System.BitConverter.ToUInt64(id.ToByteArray(), 0);
        networkId.SecondHalf = System.BitConverter.ToUInt64(id.ToByteArray(), 8);
        return networkId;
    }

    public static System.Guid ToGuid(this NetworkGuid networkId)
    {
        var bytes = new byte[16];
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(networkId.FirstHalf), 0, bytes, 0, 8);
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(networkId.SecondHalf), 0, bytes, 8, 8);
        return new System.Guid(bytes);
    }
}
