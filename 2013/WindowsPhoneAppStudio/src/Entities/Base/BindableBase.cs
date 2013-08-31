using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPAppStudio.Entities.Base
{
    /// <summary>
    /// Abstraction of data-binder that notifies properties changes to the suitable View.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event launched when a property of the bindable object has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of the binded property.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="storage">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A boolean indicating the success of the assignation.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]String propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Event handler for the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes the bindable object.
        /// </summary>
        /// <param name="parameters">Dictionary with the parameters.</param>
        public virtual void Initialize(IDictionary<string, string> parameters)
        {

        }
    }
}
