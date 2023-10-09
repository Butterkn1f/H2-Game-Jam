using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.DesignPatterns 
{
    // A pseudo-observer pattern, which allows mass calls of interfaces
    // e.g. you have a reset function which needs to be called in 10 different classes
    // put reset in an interface and mass call them with a child of this class
    public class Observer<T> : Singleton<Observer<T>>
    {
        protected List<T> _subscribers;

        override protected void Awake()
        {
            base.Awake();

            // On Awake, make sure that the list is initialised
            _subscribers = new List<T>();
        }

        public void OnDestroy()
        {
            // remove all subscribers because safety
            _subscribers.Clear();
        }

        // Add an object as a subscriber to the list -> it will get notified when an event happens
        public void AddSubscriber(GameObject GO_toAdd) 
        {
            T newComponent = GO_toAdd.GetComponent<T>();
            if (newComponent != null) 
            {
                if (!_subscribers.Contains(GO_toAdd.GetComponent<T>())) 
                {
                    _subscribers.Add(newComponent);
                }
            }
            else 
            {
                // Component to add does not exist in the current object, reject
                Debug.LogWarning("The gameobject you are trying to add does not contain" + typeof(T).FullName + ".");
            }
        }

        public void AddSubscriber(T Component_toAdd) 
        {
            if (Component_toAdd != null) 
            {
                if (!_subscribers.Contains(Component_toAdd)) 
                {
                    _subscribers.Add(Component_toAdd);
                }
            }
            else 
            {
                // Component to add does not exist in the current object, reject
                Debug.LogWarning("The gameobject you are trying to add does not exist");
            }
        }

        // Remove the object as a subscriber -> it will not get notified anymore !!
        public void RemoveSubscriber(GameObject GO_toRemove) 
        {
            if (_subscribers.Remove(GO_toRemove.GetComponent<T>()) == false) 
            {
                Debug.LogWarning("The object you are trying to remove does not exist as a subscriber. ");
            }
        }

        public void RemoveSubscriber(T Component_toRemove) 
        {
            if (_subscribers.Remove(Component_toRemove) == false) 
            {
                Debug.LogWarning("The object you are trying to remove does not exist as a subscriber. ");
            }
        }
    }
}

