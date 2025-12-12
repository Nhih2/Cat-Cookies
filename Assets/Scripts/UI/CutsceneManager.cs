using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //
        typewriter = text.GetComponent<TypewriterCore>();
        Hide();

    }
    [SerializeField] private GameObject ClickToContinue;
    [SerializeField] private GameObject text;
    [SerializeField] private Image image;

    [Header("Settings")]
    [SerializeField] private float autoDelay = 3f;

    private TypewriterCore typewriter;

    public event Action OnCutsceneEnd;

    private void Hide()
    {
        image.SetTransparency(0);
        text.SetActive(false);
        ClickToContinue.SetActive(false);
    }

    private void Show()
    {
        image.SetTransparency(1);
        text.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cutsceneRoutine != null)
        {
            if (typewriter.isShowingText)
            {
                typewriter.SkipTypewriter();
            }
            else
            {
                isWaiting = false;
            }
        }
    }

    private List<string> openingText = new()
    {
        "M-Mom. Look! I just found this <shake a=5>Angel</shake> outside our house!\nCan we keep him, please?",
        "Is that<speed=1>...<speed=20> a cat??",
        "Y-Yeah! Isn't he cute?",
        "<speed=1.5>We haven't got enough money to feed both of you,\n<speed=0.5>and now you want a <speed=0.25><shake a=6>CAT</shake>?",
        "Are you kidding me? Did I raise you to be this-",
        "Wait! Mom! I promise he will be useful!",
        "Did you just sho<speed=0.75>...\n<speed=2>Fine, you know what? You can take care of the cat.\n",
        "If it doesn't prove useful,<waitfor=2>\nthen you know what will happen.",
        "Yay! I will, thanks mom!",
        ".<waitfor=0.5>.<waitfor=0.5>.<waitfor=0.5>",
        "Meow<waitfor=1.5>\n(Look like I need to keep an eye out\nfor the mother's mood)",
        "Meow Meow\n(If her mood is <shake a=0.75>Bad</shake>,\nI will sure get kicked at the end of the day)",
        "Meow meow Meow!\n(I know! I just need to catch mouses\nand be helpful!)",
        "Meow! Meow!\n(Then I will get food to eat everyday!\nGoodbye Wandering Life! Here come my Lazy Life!)",
    },
    kickAngerText = new()
    {
        "This DAMM <speed=0.1><shake a=6>CAT!!!</shake>",
        "Get out of my <speed=0.25><shake a=6>HOUSE</shake>",
        "I've had enough of your shenanigans!",
        "Meoww...\n(Oh no, I ate too much cookies)",
         "<shake a=1>GAME OVER</shake>!\nClick to Play Again"
    },
    kickAnnoyedText = new()
    {
        "Wait! Mom! Please don't!",
        "Do or Don't with you! This cat is not one bit helpful!",
        "I've had enough of its claws!",
        "Mom!",
        "My house, my decision!",
        "Out <speed=0.25><shake a=6>NOW</shake>!",
        "Meowwwwww\n(Nooooo, my new liveeeeeee)",
        "<shake a=1>GAME OVER</shake>!\nClick to Play Again"
    },
    hardcoreText = new()
    {
        "Sarkaz, come here!",
        "Y-Yes! W-What's the matter, mom?",
        "Look at how much biscuits your stupid cat has eaten!",
        "W-What?",
        "That's it, i'm kicking it out of the house!",
        "Mom! Wait! Look!",
        "What now?",
        "Angel can do chores for you!",
        "Meow???\n(Human child, what did you say???)",
        "See? He says he can do it!\n",
        "Hm...",
        "<shake a=3>Mreeeeeeow!</shake>\n(This is cat labor abuse!)",
        "Look! He's excited to help!\nHe said that he can even cook for us!",
        "Fine,\nif the cat can do chores for me,\nthen I will allow it to stay."
    }, desireText = new()
    {
        "Meow...\n(Those cookis smell sooooo goood)",
        "Meowwwww\n(I want to take a bit\nin those colorful cookies)",
        "Meow!\n(Especially the ones in the jar!)",
        "Meow!\n(I feel like my life may change\nif I eat them!)",
        "Meow Meow...\n(Zzz)"
    };


    private int currentIndex = 0;
    private bool isWaiting = false;
    private Coroutine cutsceneRoutine;

    public void PlayCutscene_Opening(Action OnCutsceneEnd)
    {
        if (cutsceneRoutine != null) return;
        cutsceneRoutine = StartCoroutine(PlayCutsceneRoutine(openingText, OnCutsceneEnd));
    }

    public void PlayCutscene_KickedAnger(Action OnCutsceneEnd)
    {
        if (cutsceneRoutine != null) return;
        cutsceneRoutine = StartCoroutine(PlayCutsceneRoutine(kickAngerText, OnCutsceneEnd));
    }
    public void PlayCutscene_KickedAnnoyed(Action OnCutsceneEnd)
    {
        if (cutsceneRoutine != null) return;
        cutsceneRoutine = StartCoroutine(PlayCutsceneRoutine(kickAnnoyedText, OnCutsceneEnd));
    }
    public void PlayCutscene_Hardcore(Action OnCutsceneEnd)
    {
        if (cutsceneRoutine != null) return;
        cutsceneRoutine = StartCoroutine(PlayCutsceneRoutine(hardcoreText, OnCutsceneEnd));
    }

    public void PlayCutscene_Desire(Action OnCutsceneEnd)
    {
        if (cutsceneRoutine != null) return;
        cutsceneRoutine = StartCoroutine(PlayCutsceneRoutine(desireText, OnCutsceneEnd));
    }

    private IEnumerator PlayCutsceneRoutine(List<string> texts, Action OnCutsceneEnd)
    {
        currentIndex = 0;
        Show();

        while (currentIndex < texts.Count)
        {
            typewriter.ShowText(texts[currentIndex]);
            typewriter.StartShowingText();

            yield return new WaitUntil(() => !typewriter.isShowingText);

            ClickToContinue.SetActive(true);

            float timer = 0f;
            isWaiting = true;

            while (timer < autoDelay && isWaiting)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            isWaiting = false;
            currentIndex++;

            ClickToContinue.SetActive(false);
        }

        cutsceneRoutine = null;
        OnCutsceneEnd();
        Hide();
    }
}
