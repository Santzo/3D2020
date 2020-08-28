using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject heartPrefab;
    private Vector2 heartStartPoint;
    private float heartOffset;
    private MeatPrefab[] meatPrefabs;
    private Color fadedHealth = new Color(0.7f, 0.7f, 0.7f, 0.3f);
    private int _animationLoseHealth = Animator.StringToHash("LoseHealth");
    private int _animationGainHealth = Animator.StringToHash("GainHealth");
    private void Awake()
    {
        heartStartPoint = new Vector2(Screen.width * 0.03f, Screen.height - (Screen.height * 0.04f));
        heartOffset = 95f;
        Events.onPlayerHealthChange += UpdateHearts;
    }

    void UpdateHearts(int oldHealth, int newHealth, int maxHealth)
    {
        Debug.Log(oldHealth + ", " + newHealth);
        if (meatPrefabs == null)
        {
            meatPrefabs = new MeatPrefab[maxHealth].Populate();
            for (int i = 0; i < maxHealth; i++)
            {
                var go = Instantiate(heartPrefab, new Vector2(heartStartPoint.x + i * heartOffset, heartStartPoint.y), Quaternion.identity, transform);
                meatPrefabs[i].image = go.GetComponent<Image>();
                meatPrefabs[i].animator = go.GetComponent<Animator>();
                meatPrefabs[i].image.color = i < newHealth ? Color.white : fadedHealth;
            }
        }
        else
        {
            for (int i = 0; i < meatPrefabs.Length; i++)
            {
                meatPrefabs[i].image.color = i < newHealth ? Color.white : fadedHealth;
                if (i >= oldHealth && i < newHealth) meatPrefabs[i].animator.SetTrigger(_animationGainHealth);
                if (i >= newHealth && i < oldHealth) meatPrefabs[i].animator.SetTrigger(_animationLoseHealth);
            }
        }
    }
    private class MeatPrefab
    {
        public Image image;
        public Animator animator;
    }
}
