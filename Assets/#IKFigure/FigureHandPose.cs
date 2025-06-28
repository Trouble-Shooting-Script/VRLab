using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Processing;

public class FigureHandPose : MonoBehaviour
{
    private const int JOINT_PER_FINGER = 4;
    public Transform m_ModelLeftHandRootTransform;
    public Transform[] m_ModelLeftHandTransforms;
    private Vector3[] m_BaseFingerDirection = new Vector3[20];
    
    public XRHandTrackingEvents m_HandTrackingEvents;
    private List<Pose> m_LeftHandJointPoses = new List<Pose>();
    
    public bool m_isReserved = false;
    
    // Debug Joint
    public GameObject jointPrefab;
    public Transform[] debugJoints = new Transform[26];

    private void Awake()
    {
        for (int i = 0; i < m_ModelLeftHandTransforms.Length; i++)
        {
            if ((i + 1) % JOINT_PER_FINGER == 0)
            {
                m_BaseFingerDirection[i] = m_BaseFingerDirection[i - 1];
                continue;
            }
            m_BaseFingerDirection[i] = (m_ModelLeftHandTransforms[i + 1].position - m_ModelLeftHandTransforms[i].position).normalized;
        }
    }

    private void Start()
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
        //var joints = args.hand.GetRawJointArray();
        //CalculateJointTransformLocalPoses(ref joints, ref m_JointLocalPoses);
        m_LeftHandJointPoses.Clear();
        
        Pose rootPose = args.hand.rootPose;
        var inverseParentRotation = Quaternion.Inverse(rootPose.rotation);
        
        for(var i = XRHandJointID.BeginMarker.ToIndex();
            i < XRHandJointID.EndMarker.ToIndex();
            i++)
        {
            var trackingData = args.hand.GetJoint(XRHandJointIDUtility.FromIndex(i));
            if (trackingData.TryGetPose(out Pose pose))
            {
                pose.position = inverseParentRotation * (pose.position - rootPose.position);
                
                //debugJoints[i].position = Quaternion.AngleAxis(-90f, m_ModelLeftHandRootTransform.up) * pose.position + m_ModelLeftHandRootTransform.position;
                
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
        }

        Vector3[] newFingerDirection = new Vector3[20];
        for (int i = 0; i < m_ModelLeftHandTransforms.Length; i++)
        {
            if ((i + 1) % JOINT_PER_FINGER == 0)
            {
                newFingerDirection[i] = newFingerDirection[i - 1];
                continue;
            }
            newFingerDirection[i] = m_ModelLeftHandTransforms[i + 1].position - m_ModelLeftHandTransforms[i].position;
        }
        
        for(int i = 0; i < newFingerDirection.Length; i++)
        {
            Vector3 direction = newFingerDirection[i];
            if (direction != Vector3.zero)
            {
                direction.Normalize();
                m_ModelLeftHandTransforms[i].rotation = Quaternion.FromToRotation(m_BaseFingerDirection[i], direction);
            }
        }
        
        for (int i = 0; i < m_ModelLeftHandTransforms.Length; i++)
        {
            m_ModelLeftHandTransforms[i].position = Quaternion.AngleAxis(-90f, m_ModelLeftHandRootTransform.up) * m_LeftHandJointPoses[i].position + m_ModelLeftHandRootTransform.position;
        }
    }
}
