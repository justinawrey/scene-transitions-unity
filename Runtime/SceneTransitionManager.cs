using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransitions
{
    public class Tasks
    {
        public Task TransitionedOut;
        public Task NextSceneLoaded;
        public Task TransitionComplete;
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

        public static Task LoadSceneAdditiveTask(string toSceneName)
        {
            return AsyncLoader.LoadSceneAsyncTask(toSceneName, LoadSceneMode.Additive);
        }

        public static Task UnloadSceneAdditiveTask(string toSceneName)
        {
            return AsyncLoader.UnloadSceneAsyncTask(toSceneName);
        }

        public static IEnumerator LoadSceneAdditive(string toSceneName)
        {
            yield return AsyncLoader.LoadSceneAsync(toSceneName, LoadSceneMode.Additive);
        }

        public static IEnumerator UnloadSceneAdditive(string toSceneName)
        {
            yield return AsyncLoader.UnloadSceneAsync(toSceneName);
        }

        public static Tasks LoadScene(string toSceneName)
        {
            var transitionedOutTcs = new TaskCompletionSource<bool>();
            var nextSceneLoadedTcs = new TaskCompletionSource<bool>();
            var transitionCompleteTcs = new TaskCompletionSource<bool>();
            Tasks tasks = new()
            {
                TransitionedOut = transitionedOutTcs.Task,
                NextSceneLoaded = nextSceneLoadedTcs.Task,
                TransitionComplete = transitionCompleteTcs.Task,
            };

            if (_transitionObject != null)
            {
                transitionedOutTcs.SetResult(false);
                nextSceneLoadedTcs.SetResult(false);
                transitionCompleteTcs.SetResult(false);
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

        private static void OnTransitionedOut(TaskCompletionSource<bool> tcs)
        {
            tcs.SetResult(true);
        }

        private static void OnNextSceneLoaded(TaskCompletionSource<bool> tcs)
        {
            tcs.SetResult(true);
        }

        private static void OnTransitionComplete(TaskCompletionSource<bool> tcs)
        {
            GameObject.Destroy(_transitionObject);
            _transitionObject = null;
            tcs.SetResult(true);
        }
    }
}
