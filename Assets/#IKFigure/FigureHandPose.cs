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
    
    void Start()
    {
        // var handSubsystems = new List<XRHandSubsystem>();
        // SubsystemManager.GetSubsystems(handSubsystems);
        //
        // for (var i = 0; i < handSubsystems.Count; ++i)
        // {
        //     var handSubsystem = handSubsystems[i];
        //     if (handSubsystem.running)
        //     {
        //         m_HandSubsystem = handSubsystem;
        //         break;
        //     }
        // }
        //
        // if (m_HandSubsystem != null)
        //     m_HandSubsystem.updatedHands += OnUpdatedHands;
        m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    }

    // private void OnUpdatedHands(XRHandSubsystem subsystem,
    //     XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
    //     XRHandSubsystem.UpdateType updateType)
    // {
    //     switch (updateType)
    //     {
    //         case XRHandSubsystem.UpdateType.Dynamic:
    //             // Update game logic that uses hand data
    //             break;
    //         case XRHandSubsystem.UpdateType.BeforeRender:
    //             // Update visual objects that use hand data
    //             if (m_isReserved)
    //             {
    //                 UpdateJointsData(subsystem.leftHand);
    //                 ApplyPoseData();
    //                 m_isReserved = false;
    //             }
    //             break;
    //     }
    // }

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
        
        for(var i = XRHandJointID.ThumbMetacarpal.ToIndex();
            i < XRHandJointID.EndMarker.ToIndex();
            i++)
        {
            if (i is 7 or 12 or 17 or 22)
            {
                // Skip the metacarpal joints for the index, middle, ring, and little fingers
                continue;
            }
            var trackingData = args.hand.GetJoint(XRHandJointIDUtility.FromIndex(i));

            if (trackingData.TryGetPose(out Pose pose))
            {
                // 데이터는 26개 본은 20개
                pose.position = pose.position - rootPose.position;
                pose.rotation = Quaternion.Inverse(rootPose.rotation) * pose.rotation;
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
