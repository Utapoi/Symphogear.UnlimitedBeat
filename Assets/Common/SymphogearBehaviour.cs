using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Symphogear.Common
{
    /// <summary>
    /// An extended <see cref="MonoBehaviour"/> that should be used for every <see cref="Behaviour" /> in the project.
    /// </summary>
    /// <remarks>
    /// Adds useful methods that streamline the behaviour logic and architecture.
    /// Also simplify testing behaviours and methods.
    /// </remarks>

    public class SymphogearBehaviour : MonoBehaviour
    {
        /// <summary>
        /// A method that should be used to configure a component such as initializing variables or states before the
        /// application starts.
        /// </summary>
        /// <returns><c>true</c> if everything was configured; <c>false</c> otherwise.</returns>
        public virtual bool Prepare()
        {
            return true;
        }

        private void Awake()
        {
            var result = Prepare();

            if (!result)
                throw new InvalidOperationException("Failed to prepare this behaviour for usage.");
        }

        /// <summary>
        /// A method that should be used to resolve dependencies between components or behaviours.
        /// Can also be used to initialize variables or states that cannot be configured in <see cref="Prepare"/>.
        /// </summary>
        /// <returns><c>true</c> if everything was initialized; <c>false</c> otherwise.</returns>
        public virtual bool Initialize()
        {
            return true;
        }

        protected void InitializeComponent<U>(ref U component) where U : Component
        {
            if (component != null)
                return;

            component = GetOrFindComponent<U>();
        }

        protected void InitializeComponent<U>(ref U component, U fallbackValue) where U : Component
        {
            if (component != null)
                return;

            component = fallbackValue;
        }

        protected U GetOrFindComponent<U>() where U : Component
        {
            if (!TryGetComponent<U>(out var component))
                component = FindObjectOfType<U>();

            return component;
        }

        private void Start()
        {
            var result = Initialize();

            if (!result)
                throw new InvalidOperationException("Failed to initialize this behaviour.");
        }

        /// <summary>
        /// A method used to clean-up everything that should be delete before destroying the component or behaviour.
        /// </summary>
        /// <returns><c>true</c> if everything was deleted; <c>false</c> otherwise.</returns>
        public virtual bool CleanUp()
        {
            return true;
        }

        private void OnDestroy()
        {
            var result = CleanUp();

            if (!result)
                throw new InvalidOperationException("Failed to clean up this behaviour.");
        }

        /// <summary>
        /// Utility method for hiding or showing the underlying <see cref="GameObject"/>.
        /// </summary>
        /// <param name="isActive">The state to apply on the underlying <see cref="GameObject"/>.</param>
        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
