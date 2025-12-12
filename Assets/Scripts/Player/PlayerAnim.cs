using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cat;
    //[SerializeField] private List<Sprite> Frames;
    [SerializeField] private List<GameObject> FrameObjs;
    [SerializeField] private float frameRate = 0.25f;

    private bool isMoving = false;
    private float time = 0;
    private int currentFrame = 0;

    void Awake()
    {
        currentFrame = 0;
    }

    public void PlayAnim()
    {
        isMoving = true;
    }

    public void StopIdle()
    {
        isMoving = false;
        currentFrame = 0;
        SetFrame(currentFrame);
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
        if (currentFrame >= FrameObjs.Count) currentFrame = 0;
        SetFrame(currentFrame);
    }

    private void SetFrame(int frame)
    {
        for (int i = 0; i < FrameObjs.Count; i++)
        {
            if (frame == i) FrameObjs[i].SetActive(true);
            else FrameObjs[i].SetActive(false);
        }
    }
}
