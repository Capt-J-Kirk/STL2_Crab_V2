using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem.iOS;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;

    [SerializeField] private float radius = 300f;

    [SerializeField] private List<Texture> icons;
    
    [SerializeField] private List<AudioClip> audioClips;

    private List<RadialMenuEntry> entries;

    // Start is called before the first frame update
    void Start()
    {
        entries = new List<RadialMenuEntry>();
    }


    void AddEntry(string pLabel, Texture pIcon, AudioClip pAudioClip)
    {
        GameObject entry = Instantiate(entryPrefab, transform);

        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetLabel(pLabel);
        rme.SetIcon(pIcon);
        rme.SetAudio(pAudioClip);
        //rme.SetCallback(pCallback);
        
        entries.Add(rme);
    }

    public void Open()
    {
        for (int i = 0; i < 6; i++)
        {
            int num = i + 1;
            AddEntry("Button" + num.ToString(), icons[i], audioClips[i]);
        }
        Rearrage();
    }
    
    public void Close()
    {
        for (int i = 0; i < 6; i++)
        {
            RectTransform rect = entries[i].GetComponent<RectTransform>();
            GameObject entry = entries[i].gameObject;
            
            rect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InQuad).SetDelay(0.05f * i);
            rect.DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.InQuad).SetDelay(0.05f * i).onComplete = delegate
            {
                Destroy(entry);
            };
        }
        
        entries.Clear();
    }

    public void Toggle()
    {
        if (entries.Count == 0)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    void Rearrage()
    {
        float radiansOfSeperation = (Mathf.PI * 2) / entries.Count;
        for (int i = 0; i < entries.Count; i++)
        {
            float x = Mathf.Sin(radiansOfSeperation * i) * radius;
            float y = Mathf.Cos(radiansOfSeperation * i) * radius;


            RectTransform rect = entries[i].GetComponent<RectTransform>();
            
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.05f * i);
            rect.DOAnchorPos(new Vector3(x, y, 0), 0.3f).SetEase(Ease.OutQuad).SetDelay(0.05f * i);
        }
    }

    public void PlayAudioClip()
    {
        this.GetComponent<AudioSource>().Play();
        //Invoke("StopAudioClip", 5.5f);
    }
    
    void StopAudioClip()
    {
        this.GetComponent<AudioSource>().Stop();
    }
}
