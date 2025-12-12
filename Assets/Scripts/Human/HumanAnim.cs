using System.Collections.Generic;
using UnityEngine;

public class HumanAnim : MonoBehaviour
{
    [SerializeField] private SpriteRenderer human;
    [SerializeField] private List<Sprite> Frames;
    [SerializeField] private float frameRate = 0.5f;

    private bool isMoving = false;
    private float time = 0;
    private int currentFrame = 0;

    void Awake()
    {
        human.sprite = Frames[currentFrame];
    }

    public void PlayAnim()
    {
        isMoving = true;
    }

    public void StopIdle()
    {
        isMoving = false;
        currentFrame = 0;
        human.sprite = Frames[currentFrame];
    }

    public void StopAnim()
    {
        isMoving = false;
    }

    void Update()
    {
        HandleMoving();
    }

    private void HandleMoving()
    {
        if (isMoving)
        {
            time += Time.deltaTime;
            if (time > frameRate)
            {
                time = 0;
                NextFrame();
            }
        }
    }

    private void NextFrame()
    {
        currentFrame++;
        if (currentFrame >= Frames.Count) currentFrame = 0;
        human.sprite = Frames[currentFrame];
    }
}
