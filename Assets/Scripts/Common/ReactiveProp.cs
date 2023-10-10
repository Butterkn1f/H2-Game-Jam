using System.Collections.Generic;
using System;


// This class ia a container for the UniRx Reactive Property
// Put in UniRx namespace because. its good coding. i guess.
namespace UniRx.Extention
{
    public class ReactiveProp<T>
    {
        // Recap for future batch people:
        // A reactive property is a observable variable that notifies you when its value changes
        private ReactiveProperty<T> _value = new ReactiveProperty<T>();
        public IObservable<T> Value => _value;

        // Setters and getters
        public void SetValue(T newValue)
        {
            // If theres no change in state, don't change (unneccesary notification)
            if (EqualityComparer<T>.Default.Equals(_value.Value, newValue)) return;
            _value.Value = newValue;
        }

        public T GetValue()
        {
            return _value.Value;
        }
    }
}


