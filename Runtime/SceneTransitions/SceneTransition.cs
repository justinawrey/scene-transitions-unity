using System;
using System.Collections;
using System.Collections.Generic;
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
            List<SetupRoutine> setupRoutines,
            Action OnFinishCallback
        )
        {
            StartCoroutine(LoadSceneRoutine(toSceneName, setupRoutines, OnFinishCallback));
        }

        private void NotifyGameObjects(string cbName)
        {
            List<GameObject> rootObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);
            foreach (GameObject gameObject in rootObjects)
            {
                gameObject.BroadcastMessage(cbName, null, SendMessageOptions.DontRequireReceiver);
            }
        }

        private IEnumerator LoadSceneRoutine(
            string toSceneName,
            List<SetupRoutine> setupRoutines,
            Action OnFinishCallback
        )
        {
            List<IEnumerator> beforeNextSceneLoadSetupRoutines = new List<IEnumerator>();
            List<IEnumerator> afterNextSceneLoadSetupRoutines = new List<IEnumerator>();

            // preserve sort order!
            if (setupRoutines != null)
            {
                foreach (SetupRoutine routine in setupRoutines)
                {
                    if (routine.Timing == SceneTransitionCallbackTiming.BeforeNextSceneLoad)
                    {
                        beforeNextSceneLoadSetupRoutines.Add(routine.Routine);
                    }
                    else
                    {
                        afterNextSceneLoadSetupRoutines.Add(routine.Routine);
                    }
                }
            }

            NotifyGameObjects("OnTransitionOutStart");
            yield return TransitionOut();
            NotifyGameObjects("OnTransitionOutEnd");

            foreach (IEnumerator routine in beforeNextSceneLoadSetupRoutines)
            {
                yield return routine;
            }

            yield return AsyncLoader.LoadSceneAsync(toSceneName);

            foreach (IEnumerator routine in afterNextSceneLoadSetupRoutines)
            {
                yield return routine;
            }

            NotifyGameObjects("OnTransitionInStart");
            yield return TransitionIn();
            NotifyGameObjects("OnTransitionInEnd");

            OnFinishCallback();
        }
    }
}

