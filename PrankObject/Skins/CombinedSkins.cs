using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity.AttachmentTools;
using Spine;
using Spine.Unity;

public class CombinedSkins : MonoBehaviour
{
	[SpineSkin]
	public List<string> skinsToCombine;
	private Skin combinedSkin;
	[SerializeField] private bool isAddAtStart = false;

    private void Start()
    {
        if(isAddAtStart)
        {
			AddSkin();
		}
    }

    public void AddSkin()
	{
		var skeletonComponent = GetComponent<ISkeletonComponent>();
		if (skeletonComponent == null) return;
		var skeleton = skeletonComponent.Skeleton;
		if (skeleton == null) return;

		combinedSkin = combinedSkin ?? new Skin("combined");
		combinedSkin.Clear();
		foreach (string skinName in skinsToCombine)
		{
            Skin skin = skeleton.Data.FindSkin(skinName);
			if (skin != null) combinedSkin.AddAttachments(skin);
		}

		skeleton.SetSkin((Skin)null);
		skeleton.SetSkin(combinedSkin);
		skeleton.SetToSetupPose();

        IAnimationStateComponent animationStateComponent = skeletonComponent as IAnimationStateComponent;
		if (animationStateComponent != null) animationStateComponent.AnimationState.Apply(skeleton);

		if(isAddAtStart)
        {
			isAddAtStart = false;
        }
	}
}
