using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneTransitions
{
  [Serializable]
  public class SceneTransitionSetting
  {
    public string ToSceneName;
    public SceneTransition Transition;
  }

  [CreateAssetMenu(fileName = "Scene Transition Settings", menuName = "Scene Transitions/Scene Transition Settings")]
  public class SceneTransitionSettings : ScriptableObject
  {
    [SerializeField] private SceneTransition _defaultTransition;
    [SerializeField] private List<SceneTransitionSetting> _overrides = new List<SceneTransitionSetting>();

    public SceneTransition GetTransitionForScene(string sceneName)
    {
      foreach (SceneTransitionSetting sceneTransitionSetting in _overrides)
      {
        if (sceneTransitionSetting.ToSceneName == sceneName)
        {
          return sceneTransitionSetting.Transition;
        }
      }

      return _defaultTransition;
    }
  }
}