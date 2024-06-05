using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
using Daybrayk.rpg;

public abstract class CharacterCombatBase : NetworkBehaviour
{
	protected CharacterRoot root;

	[Header("Damage")]
	[SerializeField]
	protected int _maxDamage;
	public int maxDamage => _maxDamage;

	[SerializeField]
	protected Transform damageMeter;

	[Header("Debug")]
	[SerializeField]
	[ReadOnly]
	protected NetworkVariable<int> _currentDamage = new NetworkVariable<int>();

	public ulong lastDamagingClient { get; protected set; }
	public virtual int currentDamage
	{
		get { return _currentDamage.Value; }
		protected set
		{
			if (!IsServer) return;

			_currentDamage.Value = value;
			float fillPercent = (float)currentDamage / (float)maxDamage;
			damageMeter.localScale = Vector3.one.With(y: fillPercent);
		}
	}

	public bool isDrying { get; protected set; } = false;
	public bool isAlive { get; protected set; } = true;


	protected void Awake()
	{
		root = GetComponent<CharacterRoot>();
	}

	public override void OnNetworkSpawn()
	{
		if (IsServer) currentDamage = _currentDamage.Value;
		else
		{
			_currentDamage.OnValueChanged += OnDamageUpdated;
		}
	}

	public virtual void ApplyDamage(int value, ulong damagingClientId)
    {
		if (!IsServer) return;

		currentDamage = Mathf.Min(currentDamage + value, maxDamage);
		if (damagingClientId != OwnerClientId) lastDamagingClient = damagingClientId;

		if (currentDamage >= maxDamage)
		{
			Soaked();
			SoakedClientRpc();
		}
	}
	public virtual void ApplyDamage(int value)
    {
		if (!IsServer) return;

		currentDamage = Mathf.Min(currentDamage + value, maxDamage);

		if (currentDamage >= maxDamage)
		{
			Soaked();
			SoakedClientRpc();
		}
	}

	public abstract void Soaked();

	[ClientRpc]
	void SoakedClientRpc()
	{
		if (IsServer) return;
		Soaked();
	}

	public void RemoveDamage(int value)
    {
		Debug.Log($"Removing damage: {value}");
		currentDamage = Mathf.Max(currentDamage - value, 0);
    }

	[ServerRpc]
	public void RemoveDamageServerRpc(int value)
	{
		RemoveDamage(value);
	}

	public virtual void BeginDry() { }
	public virtual void CancelDry() { }

	public virtual void ResetForSpawn()
    {
		currentDamage = 0;
    }

	protected virtual void OnDamageUpdated(int previousVal, int currentVal)
	{
		float fillPercent = (float)currentDamage / (float)maxDamage;
		damageMeter.localScale = Vector3.one.With(y: fillPercent);
	}

	private void OnValidate()
	{
		_maxDamage = Mathf.Max(_maxDamage, 1);
	}

	public class PlayerSoakedEvt : GameEvent
	{
		public ulong owningClientId { get; private set; }
		public ulong damagingClientId { get; private set; }
		public bool useDamagingClientId = false;

		public PlayerSoakedEvt(ulong owningId)
		{
			owningClientId = owningId;
		}

		public PlayerSoakedEvt(ulong owningId, ulong damagingId) : this(owningId)
		{
			damagingClientId = damagingId;
			useDamagingClientId = true;
		}
	}
}