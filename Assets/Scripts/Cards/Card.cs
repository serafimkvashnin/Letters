using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Cards
{
    public class Card : MonoBehaviour
    {
        public CardData CardData { get; set; }

        public int Id { get; private set; } = id++;

        public UnityEvent OnClickEvent { get; private set; } = new UnityEvent();

        [SerializeField]
        private GameObject cardImage;

        private static int id;
        private SpriteRenderer cardSpriteRenderer;
        private SpriteRenderer cardImageSpriteRenderer;

        public Vector2 CardImageInitPosition;

        public void Start()
        {
            cardImage = transform.GetChild(0).gameObject;

            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            cardSpriteRenderer = spriteRenderers[0];
            cardImageSpriteRenderer = spriteRenderers[1];

            InitCard();
        }

        public void AppearTween()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, Constants.CardAppearDuration).SetEase(Ease.OutBounce);
        }

        public void WrongTween()
        {
            cardImage.transform
                .DOShakePosition(Constants.CardWrongDuration, 0.5f)
                .SetEase(Ease.OutBounce).OnComplete(() => cardImage.transform.position = CardImageInitPosition);
        }

        /// <summary> Populates <c>Card</c> with <c>CardData</c> properties. </summary>
        private void InitCard()
        {
            cardImageSpriteRenderer.sprite = CardData.Sprite;
            cardImage.transform.localRotation = Quaternion.Euler(CardData.Rotation);
        }

        private void OnMouseDown()
        {
            OnClickEvent.Invoke();
        }
    }
}
