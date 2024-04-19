using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void RadialMenuEntryDelegate(RadialMenuEntry pEntry);
    
    [SerializeField] private TextMeshProUGUI label;
    
    [SerializeField] private RawImage icon;
    
    [SerializeField] private AudioClip audioClip;

    private RectTransform rect;
    private RadialMenuEntryDelegate Callback;

    private void Start()
    {
        rect = icon.GetComponent<RectTransform>();
    }

    public void SetLabel(string pText)
    {
        label.text = pText;
    }
    
    public void SetIcon(Texture pIcon)
    {
        icon.texture = pIcon;
    }
    
    public Texture GetIcon()
    {
        return (icon.texture);
    }
    
    public void SetAudio(AudioClip pAudioClip)
    {
        audioClip = pAudioClip;
    }
    
    public AudioClip GetAudio()
    {
        return (audioClip);
    }
    
    public void SetCallback(RadialMenuEntryDelegate pCallback)
    {
        Callback = pCallback;
    }
    
    public void PlayAudioClip()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = GetAudio();
        audio.Play();
        Debug.Log("Plays Audio  Radial Menu Entry");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button Clicked!");
        Callback?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOComplete();
        rect.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOComplete();
        rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }
}
