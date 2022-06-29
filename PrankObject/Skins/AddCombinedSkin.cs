using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using DG.Tweening;

public class AddCombinedSkin : MonoBehaviour
{
    [SerializeField] CombinedSkins combinedSkins = null;
    [SerializeField] private float delayAddSkin = 0.6f;
    [SerializeField] private bool isObjectWithoutSkin = false;
    [SerializeField] private bool isObjectWithSkin = false;
    [SerializeField] private string nameSkin = null;
    [SerializeField] private string audioDataKey = null;
    [SerializeField] private string[] saluteColor = null;
    private MixAndMatchSkins mixAndMatchSkins;

    private string colorSkin;
    private string clothesUpSkin;
    private string clothesDownSkin;
    private string hairSkin;

    public delegate void PlaySoundClip(string _name);
    public static event PlaySoundClip PlayClipEvent;

    private void Start()
    {
        mixAndMatchSkins = FindObjectOfType<MixAndMatchSkins>();
    }

    private void OnEnable()
    {
        ItemDragHandler.GetDressedEvent += GetDressed;
    }

    private void OnDisable()
    {
        ItemDragHandler.GetDressedEvent -= GetDressed;
    }

    private void GetDressed()
    {
        if (isObjectWithoutSkin)
        {
            DOTween.Sequence()
                .AppendInterval(delayAddSkin)
                .AppendCallback(() => AddSkinAnEmptyObject(nameSkin));
        }

        if (isObjectWithSkin)
        {
            DOTween.Sequence()
                .AppendInterval(delayAddSkin)
                .AppendCallback(() => AddSkinOnClothedObjects(nameSkin));
        }
    }

    public void AddSkinAnEmptyObject(string _skinName)
    {
        if(audioDataKey == null)
        {
            PlayClipEvent("");
        }
        else
            PlayClipEvent(audioDataKey);

        if(combinedSkins.skinsToCombine.Contains(_skinName))
        {
            return;
        }
        else
        {
            combinedSkins.skinsToCombine.Add(_skinName);
            combinedSkins.AddSkin();
        }        
    }

    public void AddSkinOnClothedObjects(string _skinName)
    {
        if (audioDataKey == null)
        {
            PlayClipEvent("");
        }
        else
            PlayClipEvent(audioDataKey);

        colorSkin = mixAndMatchSkins.colorSkin;
        clothesUpSkin = mixAndMatchSkins.clothesUpSkin;
        clothesDownSkin = mixAndMatchSkins.clothesDownSkin;
        hairSkin = mixAndMatchSkins.hairSkin;

        if (combinedSkins.skinsToCombine.Count == 0)
        {
            combinedSkins.skinsToCombine.Add(colorSkin);
            combinedSkins.skinsToCombine.Add(clothesUpSkin);
            combinedSkins.skinsToCombine.Add(clothesDownSkin);
            combinedSkins.skinsToCombine.Add(hairSkin);
            combinedSkins.skinsToCombine.Add(_skinName);
            combinedSkins.AddSkin();
            gameObject.GetComponent<AddCombinedSkin>().enabled = false;
        }
        else
        {
            combinedSkins.skinsToCombine.Add(_skinName);
            combinedSkins.AddSkin();
            gameObject.GetComponent<AddCombinedSkin>().enabled = false;
        }
    }

    public void RemoveSkin(string _name)
    {
        combinedSkins.skinsToCombine.Remove(_name);
        combinedSkins.AddSkin();
    }

    public void AddSaluteSkin(int _color)
    {
        for (int i = 1; i < _color; i++)
        {
            int temp = i;
            Sequence mySequence = DOTween.Sequence();
            mySequence
                .AppendInterval(temp)
                .AppendCallback(() =>
                {
                    combinedSkins.skinsToCombine.Add(saluteColor[temp]);
                    combinedSkins.AddSkin();
                });
        }
    }
}
