using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Daybrayk;
using TMPro;
[RequireComponent(typeof(NetworkObject))]
public class CharacterRoot : NetworkBehaviour
{
    [SerializeField]
    PersistentPlayerRuntimeCollection persistentPlayers;
    //[SerializeField]
    //protected AssetReferenceT<PersistentPlayerRuntimeCollection> persistentPlayerRuntimeAsset;
    //AsyncOperationHandle<PersistentPlayerRuntimeCollection> handle;


    [SerializeField]
    CharacterMovement _movement;
    public CharacterMovement movement => _movement;
    public bool hasMovement { get; private set; }

    [SerializeField]
    CharacterInputHandler _input;
    public CharacterInputHandler input => _input;
    public bool hasInput { get; private set; }

    [SerializeField]
    CharacterWeapon _weapon;
    public CharacterWeapon weapon => _weapon;
    public bool hasWeapon { get; private set; }

    [SerializeField]
    CharacterCombatBase _combat;
    public CharacterCombatBase combat => _combat;
    public bool hasCombat { get; private set; }

    [SerializeField]
    CharacterVisualization _visualization;
    public CharacterVisualization visualization => _visualization;
    public bool hasVisualization { get; private set; }

    [SerializeField]
    CharacterCameraController _camera;
    new public CharacterCameraController camera => _camera;
    public bool hasCamera { get; private set; }

    [SerializeField]
    CharacterFlagHolder _flag;
    public CharacterFlagHolder flag => _flag;
    public bool hasFlagHolder { get; private set; }

    [SerializeField]
    bool _isPlayerControlled = true;
    public bool bIsPlayerControlled => _isPlayerControlled;

    [Space(10)]

    [SerializeField]
    [ReadOnly]
    Collider2D _collider;
    new public Collider2D collider => _collider;

    [SerializeField]
    TMP_Text nameDisplay;

    PersistentPlayer _owningPlayer;
    public PersistentPlayer owningPlayer => _owningPlayer;

    public bool isResetToSpawn;

    private void Awake()
    {
        //handle = Addressables.LoadAssetAsync<PersistentPlayerRuntimeCollection>(persistentPlayerRuntimeAsset);
        //
        //handle.Completed += (operation) =>
        //{
        //    persistentPlayers = operation.Result;
        //};

        Init();
    }

    void Init()
    {
        _collider = GetComponent<Collider2D>();
        hasMovement = _movement != null;
        hasWeapon = _weapon != null;
        hasCombat = _combat != null;
        hasVisualization = _visualization != null;
        hasCamera = _camera != null;
        hasFlagHolder = _flag != null;
    }

    public override void OnNetworkSpawn()
    {
        persistentPlayers.TryGetPlayer(OwnerClientId, out _owningPlayer);

        if (IsOwner)
        {
            GameModeBase.instance.onGameOver += OnGameOver_Handler;
            //ResetToSpawnPosition();
        }

        nameDisplay.text = owningPlayer.displayName;
    }

    //private new void OnDestroy()
    //{
    //    base.OnDestroy();
    //    Addressables.Release(handle);
    //}

    public void ResetCharacter()
    {
        collider.enabled = true;
        if(hasCombat) combat.ResetForSpawn();
        if(hasWeapon) weapon.ResetForSpawn();
        if(hasVisualization ) visualization.ResetForSpawn();
    }

    public void ResetToSpawnPosition()
    {
        if (!IsOwner) return;

        if (GameModeBase.instance.TryFindAvailableSpawnPoint(owningPlayer, out PlayerSpawn spawn))
        {
            Debug.Log($"Moving player to Spawn: {spawn.transform.position}", spawn.gameObject);
            movement.rigidbody.position = spawn.transform.position;
            Debug.Log($"Player Position: {transform.position}");
            
        }
        else
        {
            Debug.Log("Could not find assigned spawn point", this);
        }
    }

    [ClientRpc]
    public void ResetToSpawnPositionClientRpc(Vector3 position)
    {
        movement.rigidbody.position = position;
        isResetToSpawn = true;
    }

    [ClientRpc]
    public void ResetCharacterClientRpc()
    {
        if (IsOwner || IsServer) return;

        ResetCharacter();
    }
    
    [ServerRpc]
    public void ResetCharacterServerRpc()
    {
        ResetCharacter();
        ResetCharacterClientRpc();
    }

    void OnGameOver_Handler()
    {
        input.DisableInput();
    }
}