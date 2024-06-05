using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
using Daybrayk.StateMachine;
public partial class GameModeBase
{
    protected SimpleStateMachine<GameModeStates> stateMachine;

    void SetupStateMachine()
    {
        stateMachine = new SimpleStateMachine<GameModeStates>();

        stateMachine.AddState(new SimpleStateMachine<GameModeStates>.SimpleState(GameModeStates.PlayersConnecting, BeginConnectingState, UpdateConnectingState, EndConnectingState));

        stateMachine.AddState(new SimpleStateMachine<GameModeStates>.SimpleState(GameModeStates.Setup, BeginSetupState, UpdateSetupState, EndSetupState));

        stateMachine.AddState(new SimpleStateMachine<GameModeStates>.SimpleState(GameModeStates.Starting, BeginStartingState, UpdateStartingState, EndStartingState));

        stateMachine.AddState(new SimpleStateMachine<GameModeStates>.SimpleState(GameModeStates.Started, BeginStartedState, UpdateStartedState, EndStartedState));

        stateMachine.AddState(new SimpleStateMachine<GameModeStates>.SimpleState(GameModeStates.Ending, BeginEndingState, UpdateEndingState, EndEndingState));

        stateMachine.StartUp();
    }

    #region Connecting State
    protected virtual void BeginConnectingState() { }

    protected virtual void UpdateConnectingState()
    {
        if (IsServer && SceneTransitionHandler.instance.AllClientsAreLoaded())
        {
            //ChangeState(GameModeStates.Setup);
            stateMachine.ChangeState(GameModeStates.Setup);
        }
    }

    protected virtual void EndConnectingState() { }
    #endregion

    #region Setup State
    protected virtual void BeginSetupState()
    {
        StartCoroutine(SetupStateHelper());
    }

    protected virtual void UpdateSetupState() { }

    protected virtual void EndSetupState()
    {
        GameEvents.instance.Raise<ScreenFader.ScreenFadeFromBlackEvt>(this);

    }
    #endregion

    #region Starting
    protected virtual void BeginStartingState()
    {
        delayTimerText.gameObject.SetActive(true);
        timer = startDelay;
    }

    protected virtual void UpdateStartingState()
    {
        timer -= Time.deltaTime;
        delayTimerText.text = Mathf.Max(0,Mathf.CeilToInt(timer)).ToString();
        if (IsServer && timer <= 0) stateMachine.ChangeState(GameModeStates.Started);
    }

    protected virtual void EndStartingState()
    {
        delayTimerText.gameObject.SetActive(false);
    }
    #endregion

    #region Started
    protected virtual void BeginStartedState()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<CharacterInputHandler>().EnableInput();
        hud.ShowMessage("GO!", 3);
        if (useTimer)
        {
            timer = gameTime * 60;
            gameTimeText.gameObject.SetActive(true);
        }
    }

    protected virtual void UpdateStartedState()
    {
        if (useTimer)
        {
            if (IsServer && timer <= 0) GameOver((ulong)winner);
            gameTimeText.text = string.Format("{0}:{1}", Mathf.FloorToInt(timer / 60f).ToString("00"), (Mathf.CeilToInt(timer) % 60).ToString("00"));
            timer = Mathf.Max(0, timer - Time.deltaTime);
        }
    }

    protected virtual void EndStartedState() { }
    #endregion

    #region Ending
    protected virtual void BeginEndingState()
    {
        //scores.Dispose();
        if (gameOverUI == null) return;
        
        gameOverUI.ShowUI();

        if (instance is TeamGameMode) gameOverUI.SetWinnerText(((PersistentPlayer.TeamColors)winner).ToString());
        else if(persistentPlayerRuntimeCollection.TryGetPlayer((ulong)winner, out var playerRuntime))
        {
            gameOverUI.SetWinnerText(playerRuntime.displayName);
        }


        timer = endDelay;
        delayTimerText.gameObject.SetActive(true);
        onGameOver.Invoke();
    }

    protected virtual void UpdateEndingState()
    {
        timer -= Time.deltaTime;
        delayTimerText.text = Mathf.CeilToInt(timer).ToString();
        if (IsServer && timer <= 0) SceneTransitionHandler.instance.SwitchScene("New Lobby");
    }

    protected virtual void EndEndingState() { }
    #endregion

    void OnStateChanged(GameModeStates value)
    {
        serverCurrentState.Value = value;
    }

    void ServerState_OnValueChanged(GameModeStates oldValue, GameModeStates newValue)
    {

        stateMachine.ChangeState(newValue);
    }

    protected virtual IEnumerator SetupStateHelper()
    {
        WaitForSeconds interval = new WaitForSeconds(0.5f);

        yield return interval;

        SetupScores();

        if (IsServer)
        {
            yield return StartCoroutine(SpawnPlayerAvatars());

            PlacePlayerAvatars();

            stateMachine.ChangeState(GameModeStates.Starting);
        }
        else
        {
            yield return new WaitUntil(() => serverCurrentState.Value == GameModeStates.Starting);
        }

        //ChangeState(GameModeStates.Starting);
    }
}
