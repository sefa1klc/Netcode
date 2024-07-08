using System;

public struct RelayJoinData
{
    public string JoinCode;
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionsData;
    public byte[] HostConnectionsData;
    public byte[] Key;
}
