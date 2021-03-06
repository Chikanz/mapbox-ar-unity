﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;
using Mapbox.Unity.Map;
using Mapbox.Utils;

[RequireComponent(typeof(AbstractMap))]
public abstract class Route : ARBase {

    protected List<Vector2d> latLon = new List<Vector2d>(); //List of lat lon coordinates
    protected List<Vector3> Route3d = new List<Vector3>(); //List of lat long converted to 3d positions in the scene

    protected Transform root; //root of objects to be spawned

    public string RouteType = "New Route"; //Name of this route

    protected AbstractMap map; //Map to get data from

    public bool hasEndMarker; //End marker
    public GameObject endMarker;

    protected List<GameObject> Objects = new List<GameObject>(); //Objects this route spawns

    private static GameObject Arrow;

    // Use this for initialization
    protected virtual void OnEnable ()
    {
        //Invoke("Init", 0.5f); //slight delay to allow mapbox to init
        map = GetComponent<MapAtWorldScale>();
        Init();

        if (!Arrow) Arrow = (GameObject) Resources.Load("Arrow");
        SpawnDirectionArrow();
    }

    /// <summary>
    /// Converts lat long points to 3d coordinates for children to use
    /// </summary>
    private void Init()
    {
        //Add coords to list
        AddCoordinates();

        //Create child
        root = Instantiate(new GameObject(RouteType), transform).transform;

        //Create points in 3D space
        for (int i = 0; i < latLon.Count; i++)
        {
            var pos = ZacMath.PlaceByLatLong(map, latLon[i], 0);
            Route3d.Add(pos);
        }

        InitRoute();

        //Spawn end marker
        SpawnEndMarker();
    }

    /// <summary>
    /// Route spawning implementation called after init()
    /// </summary>
    protected abstract void InitRoute();

    /// <summary>
    /// Route coordinates should be added here
    /// </summary>
    protected virtual void AddCoordinates()
    {
        //leave blank for children classes 
    }

    public void AddCoordinate(double lat, double lon)
    {
        latLon.Add(new Vector2d(lat, lon));
    }

    protected virtual void CleanUp()
    {
        for(int i = 0; i < Objects.Count; i++)
        {
            Destroy(Objects[i]);
        }

        latLon.Clear();
    }

    protected virtual GameObject SpawnEndMarker()
    {
        if (hasEndMarker)
        {
            return Instantiate(endMarker, Route3d[Route3d.Count - 1], Quaternion.identity, root);
        }
        return null;
    }

    private void OnDisable()
    {
        CleanUp();
        if (root) Destroy(root.gameObject);
    }

    private void SpawnDirectionArrow()
    {
        var cam = GetCamera();
        var arrow = Instantiate(Arrow, cam.transform.position + (cam.transform.forward * 7) + (cam.transform.up * 1), Quaternion.identity);
        arrow.transform.parent = GetCamera().transform;
        var face = arrow.GetComponent<FaceTowards>();
        face.DestroyOnAlign = true;
        face.Target = Route3d[0];
        face.ARCam = GetCamera().transform;
        Objects.Add(arrow);
    }
}
