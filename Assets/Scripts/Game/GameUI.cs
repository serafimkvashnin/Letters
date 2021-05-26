using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameUI : MonoBehaviour
    {
        public Game Game;
        public UnityEvent OnFadeInComplete { get; private set; } = new UnityEvent();
        public UnityEvent OnFadeOutComplete { get; private set; } = new UnityEvent();

        public GameObject FindPanel;

        [HideInInspector]
        public Text FindText;

        [HideInInspector]
        public Image FindIcon;

        public Image RestartFade;
        public Image GameFade;

        public Button Restart;

        public void Awake()
        {
            FindText = FindPanel.GetComponentInChildren<Text>();
            FindIcon = FindPanel.GetComponentInChildren<Image>();
        }

        public void Start()
        {
            Game.OnGameStart.AddListener(() => FadeInFindPanel());
            Game.OnGameEnd.AddListener(() => FadeInRestartPanel());

            Restart.onClick.AddListener(() => FadeOutGame(() => SceneManager.LoadScene(Constants.LoadingScene)));
        }

        private void FadeInFindPanel()
        {
            FindIcon.color = new Color(1f, 1f, 1f, 0f);
            FindText.color = new Color(1f, 1f, 1f, 0f);
            FindIcon.DOFade(1f, Constants.FindPanelFadeDuration).SetEase(Ease.Linear);
            FindText.DOFade(1f, Constants.FindPanelFadeDuration).SetEase(Ease.Linear);
        }

        private void FadeOutGame(Action callback)
        {
            GameFade.gameObject.SetActive(true);
            GameFade.color = Color.clear;
            GameFade.DOFade(1f, Constants.GameFadeDuration).SetEase(Ease.Linear).OnComplete(() => callback.Invoke());
        }

        private void FadeInRestartPanel()
        {
            RestartFade.gameObject.SetActive(true);
            RestartFade.color = Color.clear;
            RestartFade.DOFade(0.5f, Constants.RestartPanelFadeDuration).SetEase(Ease.Linear);

            Restart.gameObject.SetActive(true);
        }
    }
}
