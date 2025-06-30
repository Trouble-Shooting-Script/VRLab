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
    public Transform m_LeftHandRoot;
    public Transform[] m_LeftHandJoints;
    private Vector3[] m_BaseFingerDirection = new Vector3[20];
    
    public XRHandTrackingEvents m_HandTrackingEvents;
    private List<Pose> m_LeftHandJointPoses = new List<Pose>();
    
    // Debug Joint
    public GameObject jointPrefab;
    public Transform[] debugJoints = new Transform[26];

    private void Awake()
    {
        for (int i = 0; i < m_LeftHandJoints.Length; i++)
        {
            if ((i + 1) % JOINT_PER_FINGER == 0)
            {
                m_BaseFingerDirection[i] = m_BaseFingerDirection[i - 1];
                continue;
            }
            m_BaseFingerDirection[i] = (m_LeftHandJoints[i + 1].position - m_LeftHandJoints[i].position).normalized;
        }
    }

    private void Start()
    {
        if (m_HandTrackingEvents == null)
        {
            m_HandTrackingEvents = FindObjectOfType<XRHandTrackingEvents>();
        }
        m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
        
        // debug
        for (int i = 0; i < debugJoints.Length; i++)
        {
            debugJoints[i] = Instantiate(jointPrefab).transform;
        }
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        UpdateJointsData(args);
        ApplyPoseData();
    }
    
    private void UpdateJointsData(XRHandJointsUpdatedEventArgs args)
    {
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
                pose.rotation = inverseParentRotation * pose.rotation;
                
                debugJoints[i].position = Quaternion.AngleAxis(-90f, m_LeftHandRoot.up) * pose.position + m_LeftHandRoot.position;
                debugJoints[i].rotation = Quaternion.AngleAxis(-90f, m_LeftHandRoot.up) * pose.rotation;
                
                if (i is 0 or 1 or 6 or 11 or 16 or 21)
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
        
        for (int i = 0; i < m_LeftHandJoints.Length; i++)
        {
            m_LeftHandJoints[i].position = Quaternion.AngleAxis(-90f, m_LeftHandRoot.up) * m_LeftHandRoot.rotation * m_LeftHandJointPoses[i].position + m_LeftHandRoot.position;
        }

        Vector3[] newFingerDirection = new Vector3[20];
        for (int i = 0; i < m_LeftHandJoints.Length; i++)
        {
            if ((i + 1) % JOINT_PER_FINGER == 0)
            {
                newFingerDirection[i] = newFingerDirection[i - 1];
                continue;
            }
            newFingerDirection[i] = m_LeftHandJoints[i + 1].position - m_LeftHandJoints[i].position;
        }
        
        for(int i = 0; i < newFingerDirection.Length; i++)
        {
            Vector3 direction = newFingerDirection[i];
            direction.Normalize();

            if (i is 0 or 4 or 8 or 12 or 16)
            {
                m_LeftHandJoints[i].localRotation = Quaternion.FromToRotation(m_BaseFingerDirection[i], direction);
            }
            else
            {
                m_LeftHandJoints[i].localRotation = Quaternion.FromToRotation(newFingerDirection[i - 1].normalized, direction);
            }
        }
        
        for (int i = 0; i < m_LeftHandJoints.Length; i++)
        {
            m_LeftHandJoints[i].position = Quaternion.AngleAxis(-90f, m_LeftHandRoot.up) * m_LeftHandRoot.rotation * m_LeftHandJointPoses[i].position + m_LeftHandRoot.position;
        }
    }
}
