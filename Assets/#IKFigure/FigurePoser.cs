using System.Collections;
using System.Collections.Generic;
using RootMotion;
using RootMotion.FinalIK;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject poseEditMenuPrefab;
    
    public FullBodyBipedIK fbb;
    private BipedReferences ikRef;
    private List<Transform> bones = new List<Transform>();
    private List<IKTarget> ikTargets = new List<IKTarget>();
    private GameObject rootObject;
    private GameObject ikTargetRoot;
    
	private void Start()
	{
        InitializeIKTargets(fbb);
	}

    public void InitializeIKTargets(FullBodyBipedIK fullBodyBipedIK)
    {
        bones.Clear();
        ikTargets.Clear();
        
        fbb = fullBodyBipedIK;
        ikRef = fbb.references;
        InitializeIKWeights();

        rootObject = new GameObject("PoseEditor");
        rootObject.transform.SetParent(fbb.transform);
        
        GameObject menu = Instantiate(poseEditMenuPrefab, rootObject.transform);
        menu.name = "Menu";
        menu.GetComponent<FollowTransform>().target = ikRef.head;
        
        var b = rootObject.GetComponentInChildren<Button>(true);
        b.onClick.AddListener(ToggleIKTargets);
        
        ikTargetRoot = new GameObject();
        ikTargetRoot.name = "IKTargets";
        ikTargetRoot.transform.SetParent(rootObject.transform);
        
        bones.Add(fbb.solver.rootNode);
        
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
            IKTarget ikTarget = Instantiate(ikTargetPrefab, ikTargetRoot.transform);
            ikTarget.bindingBone = bone;
            ikTarget.name = BONE_NAMES[i];
            ikTarget.transform.SetWorldPose(bone.GetWorldPose());
            ikTargets.Add(ikTarget);
            ikTarget.gameObject.SetActive(false);
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

    private void InitializeIKWeights()
    {
        var solver = fbb.solver;
        var effectors = solver.effectors;
        var chains = solver.chain;
        var mappings = fbb.solver.limbMappings;

        foreach (var ikEffector in effectors)
        {
            ikEffector.maintainRelativePositionWeight = 0f;
            ikEffector.positionWeight = 1f;
            ikEffector.rotationWeight = 1f;
        }

        foreach (var fbikChain in chains)
        {
            fbikChain.bendConstraint.weight = 1f;
            fbikChain.pull = 0f;
        }

        foreach (var ikMappingLimb in mappings)
        {
            ikMappingLimb.weight = 1f;
            ikMappingLimb.maintainRotationWeight = 0f;
        }
    }

    public void ToggleIKTargets()
    {
        //ikTargetRoot.SetActive(!ikTargetRoot.activeSelf);
        foreach (IKTarget ikTarget in ikTargets)
        {
            ikTarget.gameObject.SetActive(!ikTarget.gameObject.activeSelf);
        }
    }
}
