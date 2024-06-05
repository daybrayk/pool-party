using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;

    float delay = 2f;
    float fadeDuration = 1f;
    float timer;
    Color color = new Color();

    public void Init(float delay, float fadeDuration)
    {
        this.delay = delay;
        this.fadeDuration = fadeDuration;
        color = sprite.color;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (timer > delay)
        {
            color.a = Mathf.SmoothStep(color.a, 0, timer / (fadeDuration + delay));
            sprite.color = color;
            if (color.a == 0) Destroy(gameObject);
        }

        timer += Time.deltaTime;
    }
}