using System.Collections;
using UnityEngine;

namespace SceneTransitions
{
  public static class SceneTransitionManager
  {
    private static GameObject _sceneTransitionOverlayPrefab;
    private static GameObject _transitionObject;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnAfterSceneLoad()
    {
      _sceneTransitionOverlayPrefab = Resources.Load<GameObject>("Prefabs/scene-transition-overlay");
    }

    // setupRoutine is executed after the new scene is loaded, but before the fade in transition
    public static void LoadScene(string toSceneName, IEnumerator setupRoutine = null)
    {
      if (_transitionObject != null)
      {
        return;
      }

      _transitionObject = GameObject.Instantiate(_sceneTransitionOverlayPrefab, Vector3.zero, Quaternion.identity);
      _transitionObject.GetComponent<SceneTransitionOverlay>().LoadScene(toSceneName, setupRoutine, OnTransitionComplete);
    }

    private static void OnTransitionComplete()
    {
      GameObject.Destroy(_transitionObject);
      _transitionObject = null;
    }
  }
}