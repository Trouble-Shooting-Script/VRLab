using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Hands;

public class FigureHandPose : MonoBehaviour
{
    //public XRHandSubsystem m_HandSubsystem;
    public XRHandTrackingEvents m_HandTrackingEvents;
    public Transform m_ModelLeftHandRootTransform;
    public Transform[] m_ModelLeftHandTransforms;
    private List<Pose> m_LeftHandJointPoses = new List<Pose>();
    public bool m_isReserved = false;
    
    
    // Debug Joint
    public GameObject jointPrefab;
    public Transform[] debugJoints = new Transform[26];
    
    void Start()
    {
        m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
        for (int i = 0; i < debugJoints.Length; i++)
        {
            debugJoints[i] = Instantiate(jointPrefab).transform;
        }
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        UpdateJointsData(args);
        ApplyPoseData();
        m_isReserved = false;
    }
    
    private void UpdateJointsData(XRHandJointsUpdatedEventArgs args)
    {
        m_LeftHandJointPoses.Clear();
        
        Pose rootPose = args.hand.rootPose;
        
        for(var i = XRHandJointID.BeginMarker.ToIndex();
            i < XRHandJointID.EndMarker.ToIndex();
            i++)
        {
            var trackingData = args.hand.GetJoint(XRHandJointIDUtility.FromIndex(i));
            if (trackingData.TryGetPose(out Pose pose))
            {
                pose.position = pose.position - rootPose.position;
                pose.rotation = Quaternion.Inverse(rootPose.rotation) * pose.rotation;
                
                debugJoints[i].position = Quaternion.AngleAxis(-90f, m_ModelLeftHandRootTransform.up) * pose.position + m_ModelLeftHandRootTransform.position;
                debugJoints[i].rotation = m_ModelLeftHandRootTransform.rotation * pose.rotation;
                
                if (i is 1 or 2 or 6 or 11 or 16 or 21)
                {
                    // Skip the metacarpal joints for the index, middle, ring, and little fingers
                    continue;
                }
                m_LeftHandJointPoses.Add(pose);
            }
        }
    }

    private void ApplyPoseData()
    {
        for (int i = 0; i < m_ModelLeftHandTransforms.Length; i++)
        {
            m_ModelLeftHandTransforms[i].position = Quaternion.AngleAxis(-90f, m_ModelLeftHandRootTransform.up) * m_LeftHandJointPoses[i].position + m_ModelLeftHandRootTransform.position;
            m_ModelLeftHandTransforms[i].rotation = m_ModelLeftHandRootTransform.rotation * m_LeftHandJointPoses[i].rotation;
        }
    }
}
