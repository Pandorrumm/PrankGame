using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;

public class MixAndMatchSkinsButton : MonoBehaviour
{
	public SkeletonDataAsset skeletonDataAsset;
	public MixAndMatchSkins skinsSystem;

	[SpineSkin(dataField: "skeletonDataAsset")] public string itemSkin;
	public MixAndMatchSkins.ItemType itemType;

	const string AUDIO_CLOTHE = "Clothe";

	public delegate void PlaySoundClip(string _name);
	public static event PlaySoundClip PlayClipEvent;


	public void ToClothe()
	{
		skinsSystem.Equip(itemSkin, itemType);
		PlayClipEvent(AUDIO_CLOTHE);
	}
}

