// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using System.Collections;
using Symphogear.Common;
using UnityEngine;

namespace Symphogear.Score
{
    /// <summary>
    /// A <see cref="SymphogearBehaviour"/> managing the lifetime of an accuracy prefab.
    /// </summary>
    public class Accuracy : SymphogearBehaviour
    {
        /// <summary>
        /// The lifetime of the prefab.<br/>
        /// The <see cref="GameObject" /> is destroyed after this duration.
        /// </summary>
        public float Lifetime;

        /// <inheritdoc cref="SymphogearBehaviour.Initialize"/>
        public override bool Initialize()
        {
            StartCoroutine(DestroyAfter(Lifetime));

            return base.Initialize();
        }

        /// <summary>
        /// A coroutine that destroy the underlying <see cref="GameObject"/> after a certain time.
        /// </summary>
        /// <param name="duration">The given amount of seconds used to suspend the coroutine.</param>
        /// <returns>An <see cref="IEnumerator"/> used by <see cref="MonoBehaviour.StartCoroutine"/></returns>
        private IEnumerator DestroyAfter(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
