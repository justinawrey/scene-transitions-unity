using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransitions
{
    public enum SceneTransitionCallbackTiming
    {
        BeforeNextSceneLoad,
        AfterNextSceneLoad,
    }

    public class SetupRoutine
    {
        private IEnumerator _routine;
        private SceneTransitionCallbackTiming _timing;

        public IEnumerator Routine => _routine;
        public SceneTransitionCallbackTiming Timing => _timing;

        public SetupRoutine(IEnumerator routine, SceneTransitionCallbackTiming timing)
        {
            _routine = routine;
            _timing = timing;
        }
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
        public static void OnAfterAssembliesLoaded()
        {
            TryLoadSettings();
        }

        public static IEnumerator LoadSceneAdditive(string toSceneName)
        {
            yield return AsyncLoader.LoadSceneAsync(toSceneName, LoadSceneMode.Additive);
        }

        public static IEnumerator UnloadSceneAdditive(string toSceneName)
        {
            yield return AsyncLoader.UnloadSceneAsync(toSceneName);
        }

        public static Task<bool> LoadScene(
            string toSceneName,
            List<SetupRoutine> setupRoutines = null
        )
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            if (_transitionObject != null)
            {
                taskCompletionSource.SetResult(false);
                return taskCompletionSource.Task;
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
                    setupRoutines,
                    () => OnTransitionComplete(taskCompletionSource)
                );

            return taskCompletionSource.Task;
        }

        private static void OnTransitionComplete(TaskCompletionSource<bool> tcs)
        {
            GameObject.Destroy(_transitionObject);
            _transitionObject = null;
            tcs.SetResult(true);
        }
    }
}
