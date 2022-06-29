using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity.AttachmentTools;
using Spine;
using Spine.Unity;

public class MixAndMatchSkins : MonoBehaviour
{
	[SpineSkin] public string[] colorSkins = null;
	private int activeColorIndex = 0;
	[SpineSkin] public string[] clothesUpSkins = null;
	private int activeClothesUpIndex = 0;
	[SpineSkin] public string[] clothesDownSkins = null;
	private int activeClothesDownIndex = 0;
	[SpineSkin] public string[] hairSkins = null;
	private int activeHairIndex = 0;

	public enum ItemType
	{
		Color,
		ClothesUp,
		ClothesDown, 
		Hair
	}

	[Header("Сurrent Сlothing")]
	[SpineSkin] public string colorSkin;
	[SpineSkin] public string clothesUpSkin;
	[SpineSkin] public string clothesDownSkin;
	[SpineSkin] public string hairSkin;

	SkeletonAnimation skeletonAnimation;
	Skin characterSkin;

	public Material runtimeMaterial;
	public Texture2D runtimeAtlas;

	private void Awake()
	{
		skeletonAnimation = this.GetComponent<SkeletonAnimation>();
	}

	private void Start()
	{
		UpdateCharacterSkin();
		UpdateCombinedSkin();
	}

	//public void NextColorSkin()
	//{
	//	activeColorIndex = (activeColorIndex + 1) % colorSkins.Length;
	//	UpdateCharacterSkin();
	//	UpdateCombinedSkin();
	//}

	//public void PrevColorSkin()
	//{
	//	activeColorIndex = (activeColorIndex + colorSkins.Length - 1) % colorSkins.Length;
	//	UpdateCharacterSkin();
	//	UpdateCombinedSkin();
	//}

	public void Equip(string itemSkin, ItemType itemType)
	{
		switch (itemType)
		{
			case ItemType.Color:
				colorSkin = itemSkin;
				break;
			case ItemType.ClothesUp:
				clothesUpSkin = itemSkin;
				break;
			case ItemType.ClothesDown:
				clothesDownSkin = itemSkin;
				break;
			case ItemType.Hair:
				hairSkin = itemSkin;
				break;

			default:
				break;
		}

		UpdateCombinedSkin();
	}

	public void OptimizeSkin()
	{
		var previousSkin = skeletonAnimation.Skeleton.Skin;
		if (runtimeMaterial)
			Destroy(runtimeMaterial);
		if (runtimeAtlas)
			Destroy(runtimeAtlas);
		Skin repackedSkin = previousSkin.GetRepackedSkin("Repacked skin", skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial, out runtimeMaterial, out runtimeAtlas);
		previousSkin.Clear();

		skeletonAnimation.Skeleton.Skin = repackedSkin;
		skeletonAnimation.Skeleton.SetSlotsToSetupPose();
		skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);

		AtlasUtilities.ClearCache();
	}

	public void UpdateCharacterSkin()
	{
		var skeleton = skeletonAnimation.Skeleton;
		var skeletonData = skeleton.Data;
		characterSkin = new Skin("character-base");

		//characterSkin.AddSkin(skeletonData.FindSkin(colorSkin));
		characterSkin.AddSkin(skeletonData.FindSkin(colorSkins[activeColorIndex]));
		characterSkin.AddSkin(skeletonData.FindSkin(clothesUpSkins[activeClothesUpIndex]));
		characterSkin.AddSkin(skeletonData.FindSkin(clothesDownSkins[activeClothesDownIndex]));
		characterSkin.AddSkin(skeletonData.FindSkin(hairSkins[activeHairIndex]));
	}

	void AddEquipmentSkinsTo(Skin combinedSkin)
	{
		var skeleton = skeletonAnimation.Skeleton;
		var skeletonData = skeleton.Data;

		if (!string.IsNullOrEmpty(clothesUpSkin)) combinedSkin.AddSkin(skeletonData.FindSkin(clothesUpSkin));
		if (!string.IsNullOrEmpty(clothesDownSkin)) combinedSkin.AddSkin(skeletonData.FindSkin(clothesDownSkin));
		if (!string.IsNullOrEmpty(colorSkin)) combinedSkin.AddSkin(skeletonData.FindSkin(colorSkin));
		if (!string.IsNullOrEmpty(hairSkin)) combinedSkin.AddSkin(skeletonData.FindSkin(hairSkin));
	}

	public void UpdateCombinedSkin()
	{
		var skeleton = skeletonAnimation.Skeleton;
		var resultCombinedSkin = new Skin("character-combined");

		resultCombinedSkin.AddSkin(characterSkin);
		AddEquipmentSkinsTo(resultCombinedSkin);

		skeleton.SetSkin(resultCombinedSkin);
		skeleton.SetSlotsToSetupPose();
	}
}
