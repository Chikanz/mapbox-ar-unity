﻿using System.Collections.Generic;
using UnityEngine;

namespace UnityARInterface
{
    public class ARPlaneVisualizer : ARBase
    {
        [SerializeField]
        private GameObject m_PlanePrefab;

        [SerializeField]
        private int m_PlaneLayer;

        public bool spawnShadowPlane = false;
        public GameObject shadowPlaneObj;
        private GameObject shadowPlane;

        public int planeLayer { get { return m_PlaneLayer; } }

        private Dictionary<string, GameObject> m_Planes = new Dictionary<string, GameObject>();

        void OnEnable()
        {
            m_PlaneLayer = LayerMask.NameToLayer ("ARGameObject");
            ARInterface.planeAdded += PlaneAddedHandler;
            ARInterface.planeUpdated += PlaneUpdatedHandler;
            ARInterface.planeRemoved += PlaneRemovedHandler;
        }

        void OnDisable()
        {
            ARInterface.planeAdded -= PlaneAddedHandler;
            ARInterface.planeUpdated -= PlaneUpdatedHandler;
            ARInterface.planeRemoved -= PlaneRemovedHandler;
        }

        protected virtual void CreateOrUpdateGameObject(BoundedPlane plane)
        {
            GameObject go;
            if (!m_Planes.TryGetValue(plane.id, out go))
            {
                go = Instantiate(m_PlanePrefab, GetRoot());

                // Make sure we can pick them later
                foreach (var collider in go.GetComponentsInChildren<Collider>())
                    collider.gameObject.layer = m_PlaneLayer;

                m_Planes.Add(plane.id, go);
            }

            //Refresh shadow plane
            if (!shadowPlane)
                shadowPlane = Instantiate(shadowPlaneObj, go.transform.position,
                    Quaternion.identity, GetRoot());

            var p = shadowPlane.transform.position;
            shadowPlane.transform.position = new Vector3(p.x, GetMedianPlaneHeight(m_Planes), p.z);


            go.transform.localPosition = plane.center;
            go.transform.localRotation = plane.rotation;
            go.transform.localScale = new Vector3(plane.extents.x, 1f, plane.extents.y);
        }

        protected virtual void PlaneAddedHandler(BoundedPlane plane)
        {
            if (m_PlanePrefab)
                CreateOrUpdateGameObject(plane);
        }

        protected virtual void PlaneUpdatedHandler(BoundedPlane plane)
        {
            if (m_PlanePrefab)
                CreateOrUpdateGameObject(plane);
        }

        protected virtual void PlaneRemovedHandler(BoundedPlane plane)
        {
            GameObject go;
            if (m_Planes.TryGetValue(plane.id, out go))
            {
                Destroy(go);
                m_Planes.Remove(plane.id);
            }
        }

        //hmmmm.....
        private float GetMedianPlaneHeight(Dictionary<string, GameObject> planes)
        {
            List<GameObject> g = new List<GameObject>();
            foreach (KeyValuePair<string, GameObject> v in planes)
            {
                g.Add(v.Value);
            }

            if (g.Count < 2)
                return g[0].transform.position.y;
            else
                return g[g.Count / 2].transform.position.y - 0.05f; //Get middle of list gameobject height
        }
    }
}
