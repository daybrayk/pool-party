using UnityEngine;
public class DisconnectReason
{
    public ConnectStatus reason { get; private set; } = ConnectStatus.Undefined;

	public void SetDisconnectReason(ConnectStatus reason)
    {
        Debug.Assert(reason != ConnectStatus.Success);
        this.reason = reason;
    }

    public void Clear()
    {
        reason = ConnectStatus.Undefined;
    }

    public bool hasTransitionReason => reason != ConnectStatus.Undefined;
}