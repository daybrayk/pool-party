using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk.StateMachine
{
    public delegate void StateDelegate();

    [System.Serializable]
    public class SimpleStateMachine<T> where T : System.Enum
	{
        public System.Action<T> stateChanged;
        public System.Action changedToDefaultState;

        public SimpleState currentState { get; private set; }

        public bool isActive { get; private set; } = false;

		Dictionary<T,SimpleState> states = new Dictionary<T, SimpleState>();

        SimpleState defaultState;

        Stack<SimpleState> stateStack;

        public SimpleStateMachine()
        {
            stateStack = new Stack<SimpleState>();
            defaultState = new SimpleState();
            currentState = defaultState;
            stateStack.Push(defaultState);
        }

        public SimpleStateMachine(SimpleState defaultState)
        {
            stateStack = new Stack<SimpleState>();
            this.defaultState = defaultState;
            ChangeToDefaultState();
            stateStack.Push(defaultState);
        }

        /// <summary>
        /// Attempst to add a state to the state machine
        /// </summary>
        /// <param name="newState"></param>
        /// <returns>Returns true if state is successfully added; returns false otherwise</returns>
        public bool AddState(SimpleState newState, bool changeToNewState = false)
        {
            if (states.ContainsKey(newState.name)) return false;

            Debug.Log($"StateMachine<{typeof(T)}>: Adding State -> {newState.name}");

            states.Add(newState.name, newState);

            if (changeToNewState) ChangeState(newState.name);

            return true;
        }

        /// <summary>
        /// Attempts to change the current state of the State Machine
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns>Returns true if successfully changes to the new state; returns false if state doens't exist or 
        /// State Machine is not active</returns>
        public bool ChangeState(T stateName)
        {
            if (currentState.name.Equals(stateName)) return false;

            if (!states.ContainsKey(stateName))
            {
                Debug.Log($"StateMachine<{typeof(T)}>: Attempted to change to non-existent state");
                return false;
            }

            Debug.Log($"StateMachine<{typeof(T)}>: Changing to state -> {stateName}");

            stateStack.Push(currentState);
            currentState.EndState();
            currentState = states[stateName];
            
            if (isActive) currentState.BeginState();

            stateChanged?.Invoke(currentState.name);
            return true;
        }

        public void ChangeToDefaultState()
        {
            currentState.EndState();
            currentState = defaultState;
            currentState.BeginState();
            changedToDefaultState?.Invoke();
        }

        public void ExitCurrentState()
        {
            if (stateStack.Count == 1) return;

            Debug.Log($"StateMachine<{typeof(T)}>: Exiting current state");

            currentState.EndState();
            currentState = stateStack.Pop();
            if(isActive) currentState.BeginState();

            stateChanged?.Invoke(currentState.name);
        }

        /// <summary>
        /// Exits the current state immediately without calling EndState. This should only be used
        /// in situations where a required condition was not met in order to properly enter the state.
        /// </summary>
        public void ExitImmediate()
        {
            if (stateStack.Count == 1) return;

            Debug.Log($"StateMachine<{typeof(T)}>: Exiting current state without calling EndState");

            currentState = stateStack.Pop();
            if (isActive) currentState.BeginState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateName"></param>
        public void RemoveState(T stateName)
        {
            if (!states.ContainsKey(stateName)) return;

            var s = states[stateName];

            states.Remove(stateName);

            if (currentState == s || states.Count == 0)
            {
                ChangeToDefaultState();
            }
        }

        public void Update()
        {
            if (!isActive) return;
        
            currentState.UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool StartUp()
        {
            if (isActive)
            {
                Debug.Log($"StateMachine<{typeof(T)}>: State Machine has already been started");
                return false;
            }

            isActive = true;
            currentState.BeginState();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            if (!isActive) return;

            currentState.EndState();

            isActive = false;
        }

        public new string ToString()
        {
            StringBuilder message = new StringBuilder($"StateMachine<{typeof(T)}>:\n");

            message.AppendLine($"Current State -> {currentState.name}");

            message.AppendLine("All States:");
            int i = 0;
            foreach (KeyValuePair<T, SimpleState> kvp in states)
            {
                message.AppendLine($"{i}. {kvp.Value.name}");
                i++;
            }

            return message.ToString();
        }
        
        [System.Serializable]
        public class SimpleState
        {
            T _name;
            public T name => _name;

            public bool hasBeginState { get; private set; } = false;
            protected StateDelegate beginState;

            public bool hasUpdateState { get; private set; } = false;
            protected StateDelegate updateState;

            public bool hasEndState { get; private set; } = false;
            protected StateDelegate endState;

            public SimpleState()
            {
                _name = default(T);
            }

            public SimpleState(T stateName)
            {
                _name = stateName;
            }

            public SimpleState(T stateName, StateDelegate begin, StateDelegate update, StateDelegate end) : this(stateName)
            {
                beginState = begin;
                updateState = update;
                endState = end;
            }

            public void BeginState()
            {
                beginState?.Invoke();
            }

            public void UpdateState()
            {
                updateState?.Invoke();
            }

            public void EndState()
            {
                endState?.Invoke();
            }

        }
        
    }
}