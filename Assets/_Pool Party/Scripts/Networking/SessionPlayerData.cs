using UnityEngine;
public struct SessionPlayerData : ISessionPlayerData
{
	public string playerName;
    public int playerNum;
    public Vector2 playerPosition;
    public bool hasCharacterSpawned;

    public bool isConnected { get; set; }
    public ulong clientId { get; set; }

    public SessionPlayerData(ulong clientId, string name, bool isConnected = false, bool hasCharacterSpawned = false)
    {
        this.clientId = clientId;
        playerName = name;
        playerNum = -1;
        playerPosition = Vector2.zero;
        this.isConnected = isConnected;
        this.hasCharacterSpawned = hasCharacterSpawned;
    }

    public void Reinitialize()
    {
        hasCharacterSpawned = false;
    }
}