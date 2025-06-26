using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RootMotion;
using RootMotion.FinalIK;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FigurePoser : MonoBehaviour
{
    private static readonly string[] BONE_NAMES = new string[]
    {
        "Root",
        "Left_Thigh",
        "Left_Calf",
        "Left_Foot",
        "Right_Thigh",
        "Right_Calf",
        "Right_Foot",
        "Left_Upper_Arm",
        "Left_Forearm",
        "Left_Hand",
        "Right_Upper_Arm",
        "Right_Forearm",
        "Right_Hand"
    };
    public IKTarget ikTargetPrefab;
    public FullBodyBipedIK fbb;
    private BipedReferences ikRef;
    private Transform rootBone;
    private List<Transform> bones = new List<Transform>();
    private List<IKTarget> ikTargets = new List<IKTarget>();
    private GameObject targetsParent;
    
    [ContextMenu("Initialize")]
    private void Initialize()
    {
        if (fbb == null)
        {
            fbb = GetComponent<FullBodyBipedIK>();
        }
        if (fbb != null)
        {
            InitializeIKTargets(fbb);
        }
    }

    public void InitializeIKTargets(FullBodyBipedIK fullBodyBipedIK)
    {
        bones.Clear();
        ikTargets.Clear();
        if (targetsParent != null)
        {
            DestroyImmediate(targetsParent);
        }
        
        fbb = fullBodyBipedIK;
        ikRef = fbb.references;

        targetsParent = new GameObject();
        targetsParent.name = "IKTargets";
        targetsParent.transform.SetParent(fbb.transform);
        
        bones.Add(ikRef.spine.Last());
        
        bones.Add(ikRef.leftThigh);
        bones.Add(ikRef.leftCalf);
        bones.Add(ikRef.leftFoot);
        bones.Add(ikRef.rightThigh);
        bones.Add(ikRef.rightCalf);
        bones.Add(ikRef.rightFoot);
        
        bones.Add(ikRef.leftUpperArm);
        bones.Add(ikRef.leftForearm);
        bones.Add(ikRef.leftHand);
        bones.Add(ikRef.rightUpperArm);
        bones.Add(ikRef.rightForearm);
        bones.Add(ikRef.rightHand);

        for (int i = 0; i < bones.Count; i++)
        {
            Transform bone = bones[i];
            IKTarget ikTarget = Instantiate(ikTargetPrefab, targetsParent.transform);
            ikTarget.bindingBone = bone;
            ikTarget.name = BONE_NAMES[i];
            ikTarget.transform.SetWorldPose(bone.GetWorldPose());
            ikTargets.Add(ikTarget);
        }

        foreach (IKTarget ikTarget in ikTargets)
        {
            IKEffector effector = fbb.solver.GetEffector(ikTarget.bindingBone);
            if (effector != null)
            {
                effector.target = ikTarget.transform;
            }
            else
            {
                FBIKChain chain = fbb.solver.GetChain(ikTarget.bindingBone);
                if (chain != null)
                {
                    chain.bendConstraint.bendGoal = ikTarget.transform;
                }
            }
        }
    }

    public void ToggleEditMode()
    {
        targetsParent.SetActive(!targetsParent.activeSelf);
    }
}
