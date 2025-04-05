using System.Collections;
using System.Threading.Tasks;
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

        private static async Task WaitForAsyncOperationTask(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                await Task.Delay(1);
            }
        }

        public static Task LoadSceneAsyncTask(
            string name,
            LoadSceneMode mode = LoadSceneMode.Single
        )
        {
            return WaitForAsyncOperationTask(SceneManager.LoadSceneAsync(name, mode));
        }

        public static Task UnloadSceneAsyncTask(string name)
        {
            return WaitForAsyncOperationTask(SceneManager.UnloadSceneAsync(name));
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
