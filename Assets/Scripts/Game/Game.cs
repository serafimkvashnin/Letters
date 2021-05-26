using Assets.Scripts.Cards;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        public GameUI GameUI;

        public UnityEvent OnGameStart { get; private set; } = new UnityEvent();
        public UnityEvent OnGameEnd { get; private set; } = new UnityEvent();

        [SerializeField]
        private GameObject cardPrefab;

        [SerializeField]
        private ParticleSystem rightAnswerParticles;

        private CardBundleData currentCardBundle;
        private CardBundleData[] cardBundles;
        private DifficultyData[] difficulties;

        private DifficultyData currentDifficulty;
        private int currentDifficultyIndex;

        private Card cardToFind;
        private Dictionary<int, Card> currentCardBundleCards;
        private Dictionary<int, Card> selectedCards;
        private Dictionary<int, Card> shownCards;

        private Dictionary<string, Dictionary<int, Card>> loadedCardBundles;

        private bool gameStarted = false;

        public void Start()
        {
            DOTween.Init();

            OnGameStart.AddListener(() => gameStarted = true);
            OnGameEnd.AddListener(() => RemoveCardOnClickListeners());

            cardBundles = Resources.LoadAll<CardBundleData>("ScriptableObjects/CardBundleData");
            difficulties = Resources.LoadAll<DifficultyData>("ScriptableObjects/DifficultyData");
            difficulties = difficulties.ToList().OrderBy(card => card.CardsToSpawn).ToArray();

            currentCardBundleCards = new Dictionary<int, Card>();
            selectedCards = new Dictionary<int, Card>();
            shownCards = new Dictionary<int, Card>();
            loadedCardBundles = new Dictionary<string, Dictionary<int, Card>>();

            SwitchLevel();
        }

        public void CardSelected(Card card)
        {
            if (cardToFind.Id == card.Id)
            {
                rightAnswerParticles.transform.position = card.transform.position;
                rightAnswerParticles.Play();
                SwitchLevel();
            } 
            else
            {
                card.WrongTween();
            }
        }

        /// <summary> Populates <value>allCards</value> from <c>CardBundleData</c>. </summary>
        private void PopulateCards()
        {
            var cardBundle = cardBundles[UnityEngine.Random.Range(0, cardBundles.Length)];

            if (currentCardBundle != null && cardBundle.name.Equals(currentCardBundle.name))
            {
                return;
            } 
            else
            {
                currentCardBundle = cardBundle;

                if (loadedCardBundles.ContainsKey(cardBundle.name))
                {
                    currentCardBundleCards = loadedCardBundles[cardBundle.name];   
                }
                else
                {
                    currentCardBundleCards.Clear();

                    foreach (var cardData in cardBundle.CardData)
                    {
                        var cardObject = Instantiate(cardPrefab);
                        cardObject.SetActive(false);

                        var cardScript = cardObject.GetComponent<Card>();
                        cardScript.CardData = cardData;

                        currentCardBundleCards.Add(cardScript.Id, cardScript);
                    }
                    loadedCardBundles.Add(cardBundle.name, currentCardBundleCards);
                }
            }
        }

        /// <summary> Populates <value>selectedCardsBuffer</value> from <value>allCards</value>. </summary>
        private void GenerateCards()
        {
            foreach (var card in selectedCards.Values)
            {
                card.gameObject.SetActive(false);
            }
            selectedCards.Clear();

            List<Card> cards = currentCardBundleCards.Values.ToList();
            float squareRoot = Mathf.Sqrt(currentDifficulty.CardsToSpawn);
            float halfSquareRoot = squareRoot / 2;  
            for (int i = 0; i < currentDifficulty.CardsToSpawn; i++)
            {
                var selectedCard = GetRandomCard(cards, selectedCards);
                selectedCard.OnClickEvent.AddListener(() => CardSelected(selectedCard));
                var cardSize = selectedCard.GetComponent<BoxCollider2D>().size;

                switch (currentDifficulty.CardsToSpawn)
                {
                    case 3:
                        selectedCard.transform.position = new Vector2(-cardSize.x + i * cardSize.x, 0);
                        break;
                    case 6:
                        selectedCard.transform.position 
                            = new Vector2(-cardSize.x + i % 3 * cardSize.x, -cardSize.y / 2 + i / 3 * cardSize.y);
                        break;
                    case 9:
                        selectedCard.transform.position 
                            = new Vector2(-cardSize.x + i % 3 * cardSize.x, -cardSize.y + i / 3 * cardSize.y);
                        break;
                    default:
                        selectedCard.transform.position = new Vector2(
                            -cardSize.x * halfSquareRoot + i / squareRoot * cardSize.x,
                            -cardSize.y * halfSquareRoot + i % squareRoot * cardSize.y
                        );
                        break;
                }

                selectedCard.CardImageInitPosition = selectedCard.transform.position;
                selectedCard.gameObject.SetActive(true);

                if (!gameStarted)
                {
                    selectedCard.AppearTween();
                }

                selectedCards.Add(selectedCard.Id, selectedCard);
            }
        }

        private void RemoveCardOnClickListeners()
        {
            foreach (var card in selectedCards.Values)
            {
                card.OnClickEvent.RemoveAllListeners();
            }
        }

        private Card GetRandomCard(List<Card> cards, Dictionary<int, Card> except = null)
        {
            Card selected = Get();
            if (except != null && except.Count > 0)
            {
                while (except.ContainsKey(selected.Id))
                {
                    selected = Get();
                }
            }
            
            Card Get()
            {
                return cards[UnityEngine.Random.Range(0, cards.Count)];
            }

            return selected;
        }

        private void SelectCardToFind()
        {
            cardToFind = GetRandomCard(selectedCards.Values.ToList(), shownCards);
            GameUI.FindIcon.sprite = cardToFind.CardData.Sprite;
            GameUI.FindIcon.transform.localRotation = Quaternion.Euler(cardToFind.CardData.Rotation);

            shownCards.Add(cardToFind.Id, cardToFind);
        }

        private void SwitchLevel()
        {
            if (currentDifficultyIndex == difficulties.Length)
            {
                OnGameEnd.Invoke();
                OnGameEnd.RemoveAllListeners();
                return;
            }

            currentDifficulty = difficulties[currentDifficultyIndex];

            PopulateCards();

            GenerateCards();

            SelectCardToFind();

            if (!gameStarted)
            {
                OnGameStart.Invoke();
            }

            currentDifficultyIndex++;
        }
    }
}
