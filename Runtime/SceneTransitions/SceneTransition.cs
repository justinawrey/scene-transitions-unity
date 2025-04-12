using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransitions
{
    // The scene loading coroutine must be done on this monobehaviour
    // because it will persist across scenes.  If we were to start the scene load
    // coroutine from the trigger object, the coroutine would die right when the new scene loads
    // and the old scene unloads.
    public abstract class SceneTransition : MonoBehaviour
    {
        protected abstract IEnumerator TransitionOut();
        protected abstract IEnumerator TransitionIn();

        protected virtual void Setup() { }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Setup();
        }

        public void LoadScene(
            string toSceneName,
            Action OnTransitionedOut,
            Action OnNextSceneLoaded,
            Action OnFinishCallback
        )
        {
            StartCoroutine(
                LoadSceneRoutine(
                    toSceneName,
                    OnTransitionedOut,
                    OnNextSceneLoaded,
                    OnFinishCallback
                )
            );
        }

        private void NotifyGameObjects(string cbName)
        {
            List<GameObject> rootObjects = new();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);
            foreach (GameObject gameObject in rootObjects)
            {
                gameObject.BroadcastMessage(cbName, null, SendMessageOptions.DontRequireReceiver);
            }
        }

        private IEnumerator LoadSceneRoutine(
            string toSceneName,
            Action OnTransitionedOut,
            Action OnNextSceneLoaded,
            Action OnFinish
        )
        {
            NotifyGameObjects("OnTransitionOutStart");
            yield return TransitionOut();
            NotifyGameObjects("OnTransitionOutEnd");

            OnTransitionedOut();

            yield return AsyncLoader.LoadSceneAsync(toSceneName);

            OnNextSceneLoaded();

            NotifyGameObjects("OnTransitionInStart");
            yield return TransitionIn();
            NotifyGameObjects("OnTransitionInEnd");

            OnFinish();
        }
    }
}
