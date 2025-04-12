using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SceneTransitions
{
    public class LoadTasks
    {
        public UniTask TransitionedOut;
        public UniTask NextSceneLoaded;
        public UniTask TransitionComplete;
    }

    public static class SceneTransitionManager
    {
        private static SceneTransitionSettings _settings;
        private static GameObject _transitionObject;

        private static void TryLoadSettings()
        {
            // try to get user defined settings first
            _settings = Resources.Load<SceneTransitionSettings>("scene-transition-settings");
            if (_settings == null)
            {
                _settings = Resources.Load<SceneTransitionSettings>(
                    "default-scene-transition-settings"
                );
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            TryLoadSettings();
        }

        public static LoadTasks LoadScene(string toSceneName)
        {
            var transitionedOutTcs = new UniTaskCompletionSource();
            var nextSceneLoadedTcs = new UniTaskCompletionSource();
            var transitionCompleteTcs = new UniTaskCompletionSource();
            LoadTasks tasks = new()
            {
                TransitionedOut = transitionedOutTcs.Task,
                NextSceneLoaded = nextSceneLoadedTcs.Task,
                TransitionComplete = transitionCompleteTcs.Task,
            };

            if (_transitionObject != null)
            {
                transitionedOutTcs.TrySetResult();
                nextSceneLoadedTcs.TrySetResult();
                transitionCompleteTcs.TrySetResult();
                return tasks;
            }

            SceneTransition transition = _settings.GetTransitionForScene(toSceneName);
            _transitionObject = GameObject.Instantiate(
                transition.gameObject,
                Vector3.zero,
                Quaternion.identity
            );
            _transitionObject
                .GetComponent<SceneTransition>()
                .LoadScene(
                    toSceneName,
                    () => OnTransitionedOut(transitionedOutTcs),
                    () => OnNextSceneLoaded(nextSceneLoadedTcs),
                    () => OnTransitionComplete(transitionCompleteTcs)
                );

            return tasks;
        }

        private static void OnTransitionedOut(UniTaskCompletionSource tcs)
        {
            tcs.TrySetResult();
        }

        private static void OnNextSceneLoaded(UniTaskCompletionSource tcs)
        {
            tcs.TrySetResult();
        }

        private static void OnTransitionComplete(UniTaskCompletionSource tcs)
        {
            GameObject.Destroy(_transitionObject);
            _transitionObject = null;
            tcs.TrySetResult();
        }
    }
}
