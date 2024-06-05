using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Daybrayk;
using Daybrayk.rpg;
public class CharacterCombat : NetworkBehaviour
{
    [Header("Damage")]
    
    [SerializeField]
    int _maxDamage = 1;
    public int maxDamage => _maxDamage;

    [SerializeField]
    Transform damageMeter;

    

    [Header("Water Trail")]
    [SerializeField]
    GameObject waterDropPrefab;
    [SerializeField]
    [MinMax]
    Vector2 dropRate;
    float dropTimer;

    [Header("Drying")]
    [SerializeField]
    float selfDryTickRate = 0.25f;
    [SerializeField]
    int selfDryTickAmount = 2;
    [SerializeField]
    StatModifier moveSpeedModifier;
    public bool isDrying { get; private set; }

    [Header("Hunter")]
    [SerializeField]
    bool isHunter;
    [SerializeField]
    ScriptableEventFloat screenFadeAlpha;
    [SerializeField]
    StatModifier soakedSpeedModifier;
    [SerializeField]
    float delay = 1f;
    float timer;
    bool isStunned = false;

    [Header("Debug")]
    [SerializeField]
    NetworkVariable<int> _currentDamage = new NetworkVariable<int>();
    public int currentDamage
    {
        get { return _currentDamage.Value; }
        private set
        {
            if (!IsServer) return;

            _currentDamage.Value = value;
            float fillPercent = (float)currentDamage / (float)maxDamage;
            damageMeter.localScale = Vector3.one.With(y: fillPercent);
            
            if (isHunter && IsOwner) screenFadeAlpha.Value = (float)currentDamage / (float)maxDamage;
        }
    }

    public bool isAlive { get; private set; } = true;
    public ulong lastDamagingClient { get; private set; }
    
    CharacterRoot root;
    Coroutine dryingCoroutine;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
        moveSpeedModifier.source = this;
    }

    private void Update()
    {
        if (!isHunter)
        {
            if (currentDamage > maxDamage / 2)
            {
                var dropTime = Mathf.Lerp(dropRate.y, dropRate.x, (float)currentDamage / (float)maxDamage);

                if (dropTimer >= dropTime)
                {
                    var drop = Instantiate(waterDropPrefab, transform.position, transform.rotation).GetComponent<WaterDrop>();
                    drop.Init(0.5f, 1f);
                    dropTimer = 0;
                }

                dropTimer += Time.deltaTime;
            }
        }
        else if(!isStunned)
        {
            if (timer <= 0) screenFadeAlpha.Value = 0;
            timer -= Time.deltaTime;
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            currentDamage = _currentDamage.Value;
        }
        else
        {
            _currentDamage.OnValueChanged += OnDamageUpdated;
        }
    }

    void OnDamageUpdated(int previousVal, int currentVal)
    {
        float fillPercent = (float)currentDamage / (float)maxDamage;
        damageMeter.localScale = Vector3.one.With(y: fillPercent);
        if (isHunter && IsOwner) screenFadeAlpha.Value = (float)currentDamage / (float)maxDamage;
    }

    /// <summary>
    /// Applies damage to the player. Only executes on the Server.
    /// </summary>
    /// <param name="value"></param>
    public virtual void ApplyDamage(int value, ulong damagingClientId)
    {
        if (!IsServer) return;

        currentDamage = Mathf.Min(currentDamage + value, maxDamage);
        
        //Neutral hazards will have a clientId < 0 and shouldn't update the lastDamagingClient
        if(damagingClientId != OwnerClientId) lastDamagingClient = damagingClientId;

        if (currentDamage >= maxDamage)
        {
            Soaked();
            SoakedClientRpc();
        }
        if (isHunter) timer = delay;
    }

    /// <summary>
    /// Applies damage to the player. Only executes on the Server.
    /// </summary>
    /// <param name="value"></param>
    public virtual void ApplyDamage(int value)
    {
        if (!IsServer) return;

        currentDamage = Mathf.Min(currentDamage + value, maxDamage);

        if (currentDamage >= maxDamage)
        {
            Soaked();
            SoakedClientRpc();
        }

        if (isHunter) timer = delay;
    }

    [ServerRpc]
    public void RemoveDamageServerRpc(int value)
    {
        RemoveDamage(value);
    }
    public void RemoveDamage(int value)
    {
        Debug.Log($"Removing damage: {value}");
        currentDamage = Mathf.Max(currentDamage - value, 0);
    }

    public void BeginDry()
    {
        if (isHunter) return;

        if (!isDrying && currentDamage > 0)
        {
            root.weapon.ChangeState(CharacterWeapon.WeaponStates.None);
            dryingCoroutine = StartCoroutine(DryHelper());
            root.movement.moveSpeedStat.AddModifier(moveSpeedModifier);
            isDrying = true;
        }
    }

    public void CancelDry()
    {
        if (!isDrying) return;

        isDrying = false;
        if(dryingCoroutine != null) StopCoroutine(dryingCoroutine);
        root.movement.moveSpeedStat.RemoveModifier(this);
        root.weapon.ExitCurrentState();
    }

    IEnumerator DryHelper()
    {
        while (currentDamage > 0)
        {
            yield return new WaitForSeconds(selfDryTickRate);
            if(IsServer) RemoveDamage(selfDryTickAmount);
            else
            {
                RemoveDamageServerRpc(selfDryTickAmount);
            }
        }

        if(root.weapon.currentState == CharacterWeapon.WeaponStates.None) root.weapon.ExitCurrentState();
        else
        {
            isDrying = false;
            root.movement.moveSpeedStat.RemoveModifier(this);

        }
    }

    public virtual void Soaked()
    {
        Debug.Log($"Player {OwnerClientId} soaked");

        if (!isHunter)
        {
            if (!isAlive) return;
            isAlive = false;
            //damageMeter.gameObject.SetActive(false);
            root.visualization.ToggleVisrep(false);
            root.weapon.StopShoot();
            root.collider.enabled = false;

            if (IsOwner)
            {
                CancelDry();
        
                StartCoroutine(RespawnHelper());
            }

            if (IsServer)
            {
                GameModeBase.instance.AdjustScore(1, lastDamagingClient, OwnerClientId);
                if (root.hasFlagHolder) root.flag.DropFlag();
                else
                {
                    Debug.Log("No Flag Holder Component");
                }
            }
        }
        else if(!isStunned && IsOwner)
        {
            Debug.Log($"Owner: {OwnerClientId}", this);
            isStunned = true;
            root.movement.moveSpeedStat.AddModifier(soakedSpeedModifier);
            StartCoroutine(StunnedHelper());
        }
    }

    IEnumerator StunnedHelper()
    {
        yield return new WaitForSeconds(2f);
        isStunned = false;
        root.movement.moveSpeedStat.RemoveModifier(soakedSpeedModifier);
        screenFadeAlpha.Value = 0;
        
        if(IsServer)RemoveDamage(maxDamage);
        else RemoveDamageServerRpc(maxDamage);
    }

    [ClientRpc]
    void SoakedClientRpc()
    {
        if (IsServer) return;
        Soaked();
    }

    IEnumerator RespawnHelper()
    {
        //root.ResetToSpawnPosition();
        GameModeBase.instance.RequestSpawnServerRpc(OwnerClientId);
        yield return new WaitWhile(() => !root.isResetToSpawn);
        yield return new WaitForSeconds(1f);
        root.isResetToSpawn = false;

        root.ResetCharacter();

        root.ResetCharacterServerRpc();
    }

    public void ResetForSpawn()
    {
        currentDamage = 0;
        isAlive = true;

    }

    private void OnValidate()
    {
        _maxDamage = Mathf.Max(_maxDamage, 1);
    }
}