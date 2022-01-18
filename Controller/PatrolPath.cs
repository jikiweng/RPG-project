﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] float waypointGizmoRadius = 0.1f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);

                Gizmos.DrawSphere(GetCurrentWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetCurrentWaypoint(i), GetCurrentWaypoint(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if (i == transform.childCount - 1)
                return 0;
            return i+1;
        }

        public Vector3 GetCurrentWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
