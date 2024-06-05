using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
    public class GameEvents
    {
        static GameEvents _instance;

        public static GameEvents instance
        {
            get
            {
                if (_instance == null) return new GameEvents();
                return _instance;
            }
        }


        public delegate void EventDelegate<T>(T e) where T : GameEvent;

        Dictionary<Type, IInvokable> events;

        public GameEvents()
        {
            _instance = this;
            events = new Dictionary<Type, IInvokable>();
        }

        public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            if(!events.ContainsKey(typeof(T))) events[typeof(T)] = new Invokable<T>();

            var invokable = events[typeof(T)] as Invokable<T>;
            invokable.del += del;
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            if (events.ContainsKey(typeof(T)))
            {
                var invokable = events[typeof(T)] as Invokable<T>;
                invokable.del -= del;
            }
        }

        public void Raise<T>(object o) where T : GameEvent
        {
            if (events.ContainsKey(typeof(T))) events[typeof(T)].Invoke(o);
        }
    }

    public interface IInvokable
    {
        void Invoke(object o);
    }

    public class Invokable<T> : IInvokable where T : GameEvent
    {
        public event GameEvents.EventDelegate<T> del;

        public void Invoke(object o)
        {
            if (del != null) del.Invoke(o as T);
        }
    }

    public abstract class GameEvent
    {
        public event Action Event;
    }
}
