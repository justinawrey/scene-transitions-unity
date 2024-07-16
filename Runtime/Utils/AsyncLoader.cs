using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransitions
{
    public static class AsyncLoader
    {
        private static IEnumerator WaitForAsyncOperation(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        public static IEnumerator LoadSceneAsync(
            string name,
            LoadSceneMode mode = LoadSceneMode.Single
        )
        {
            yield return WaitForAsyncOperation(SceneManager.LoadSceneAsync(name, mode));
        }

        public static IEnumerator UnloadSceneAsync(string name)
        {
            yield return WaitForAsyncOperation(SceneManager.UnloadSceneAsync(name));
        }
    }
}
