using UnityEngine;
using UnityEngine.UI;

public class AutoScalingText : MonoBehaviour
{
    public float LifeTime;

    private float BirthTime;

    public Text Text;

    public AnimationCurve ScaleOverLifetime;

    public AnimationCurve OpacityOverLifetime;

    public void Start()
    {
        BirthTime = Time.time;
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, 0);
    }

    public void FixedUpdate()
    {
        var percent = (Time.time - BirthTime) / LifeTime;

        var scalex = 1f;
        if (ScaleOverLifetime != null)
        {
            scalex = scalex * ScaleOverLifetime.Evaluate(percent);
        }
        transform.localScale = new Vector3(scalex, scalex, 1f);

        var opacity = 1f;
        if (ScaleOverLifetime != null)
        {
            opacity = opacity * ScaleOverLifetime.Evaluate(percent);
        }
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, opacity);

        if (BirthTime + LifeTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
