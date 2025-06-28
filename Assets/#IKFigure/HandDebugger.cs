using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;

public class HandDebugger : MonoBehaviour
{
    public XRHandTrackingEvents m_HandTrackingEvents;

    public Transform XROrigin;
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
    }

    private void UpdateJointsData(XRHandJointsUpdatedEventArgs args)
    {
        //var joints = args.hand.GetRawJointArray();
        //CalculateJointTransformLocalPoses(ref joints, ref m_JointLocalPoses);

        Pose rootPose = args.hand.rootPose;
        var inverseParentRotation = Quaternion.Inverse(rootPose.rotation);

        for (var i = XRHandJointID.BeginMarker.ToIndex();
             i < XRHandJointID.EndMarker.ToIndex();
             i++)
        {
            var trackingData = args.hand.GetJoint(XRHandJointIDUtility.FromIndex(i));
            if (trackingData.TryGetPose(out Pose pose))
            {
                debugJoints[i].SetWorldPose(pose);
                debugJoints[i].position += XROrigin.position;

            }
        }
    }
}
