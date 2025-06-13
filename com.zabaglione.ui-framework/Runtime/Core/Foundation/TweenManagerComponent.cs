using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace jp.zabaglione.ui.core.foundation
{
    /// <summary>
    /// Base component for managing DOTween animations with automatic cleanup
    /// </summary>
    public abstract class TweenManagerComponent : MonoBehaviour
    {
        private readonly List<Tween> _activeTweens = new List<Tween>();
        private bool _isPaused = false;

        /// <summary>
        /// Registers a tween for automatic lifecycle management
        /// </summary>
        /// <param name="tween">The tween to manage</param>
        /// <returns>The registered tween for chaining</returns>
        protected T RegisterTween<T>(T tween) where T : Tween
        {
            if (tween == null) return null;

            _activeTweens.Add(tween);
            tween.OnKill(() => _activeTweens.Remove(tween));

            if (_isPaused)
            {
                tween.Pause();
            }

            return tween;
        }

        /// <summary>
        /// Kills all active tweens
        /// </summary>
        /// <param name="complete">Whether to complete the tweens before killing</param>
        protected void KillAllTweens(bool complete = false)
        {
            for (int i = _activeTweens.Count - 1; i >= 0; i--)
            {
                var tween = _activeTweens[i];
                if (tween != null && tween.IsActive())
                {
                    tween.Kill(complete);
                }
            }
            _activeTweens.Clear();
        }

        /// <summary>
        /// Pauses all active tweens
        /// </summary>
        protected void PauseAllTweens()
        {
            _isPaused = true;
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && tween.IsPlaying())
                {
                    tween.Pause();
                }
            }
        }

        /// <summary>
        /// Resumes all paused tweens
        /// </summary>
        protected void ResumeAllTweens()
        {
            _isPaused = false;
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && !tween.IsPlaying())
                {
                    tween.Play();
                }
            }
        }

        /// <summary>
        /// Gets the number of active tweens
        /// </summary>
        protected int ActiveTweenCount => _activeTweens.Count;

        /// <summary>
        /// Checks if any tweens are currently playing
        /// </summary>
        protected bool HasActiveTweens()
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && tween.IsPlaying())
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void OnDestroy()
        {
            KillAllTweens();
        }

        protected virtual void OnDisable()
        {
            PauseAllTweens();
        }

        protected virtual void OnEnable()
        {
            ResumeAllTweens();
        }

        /// <summary>
        /// Creates a safe delay that's automatically managed
        /// </summary>
        protected Tween Delay(float duration, TweenCallback callback)
        {
            return RegisterTween(DOVirtual.DelayedCall(duration, callback));
        }

        /// <summary>
        /// Creates a safe sequence that's automatically managed
        /// </summary>
        protected Sequence CreateSequence()
        {
            return RegisterTween(DOTween.Sequence());
        }
    }
}