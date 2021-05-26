using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Text progressText;

    private AsyncOperation asyncLoading;

    void Start()
    {
        asyncLoading = SceneManager.LoadSceneAsync(Constants.GameScene);
    }

    void Update()
    {
        float progressValue = Mathf.Clamp01(asyncLoading.progress / 0.9f);
        progressText.text = $"Loading... {Mathf.Round(progressValue * 100)}%";
    }
}
