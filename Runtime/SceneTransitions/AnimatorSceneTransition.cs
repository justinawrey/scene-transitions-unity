using System.Collections;
using UnityEngine;

namespace SceneTransitions
{
    public class AnimatorSceneTransition : SceneTransition
    {
        private Animator _animator;

        private bool _transitionedIn = false;
        private bool _transitionedOut = false;

        protected override void Setup()
        {
            _animator = GetComponent<Animator>();
        }

        protected override IEnumerator TransitionIn()
        {
            _animator.SetBool("SceneVisible", true);
            yield return new WaitUntil(() => _transitionedIn);
            _transitionedIn = false;
        }

        protected override IEnumerator TransitionOut()
        {
            _animator.SetBool("SceneVisible", false);
            yield return new WaitUntil(() => _transitionedOut);
            _transitionedOut = false;
        }

        // called by animation event
        public void OnTransitionInEnd()
        {
            _transitionedIn = true;
        }

        // called by animation event
        public void OnTransitionOutEnd()
        {
            _transitionedOut = true;
        }
    }
}
