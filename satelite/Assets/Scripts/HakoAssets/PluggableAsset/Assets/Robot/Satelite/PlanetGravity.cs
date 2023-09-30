using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using UnityEngine;

public class PlanetGravity : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
{
    private GameObject root;
    private static float M = 5.972f * Mathf.Pow(10, 10);

    public GameObject Satelite = null;
    public GameObject Planet = null;

    private Rigidbody SateliterRig = null;

    private Vector3 f;
    private float m;
    private float r;
    public float G = 6.67430f * Mathf.Pow(10, -11);

    public void Initialize(object root)
    {
        if (this.root != null)
        {
            return;
        }
        this.root = (GameObject)root;
        SateliterRig = Satelite.GetComponent<Rigidbody>();
        m = Satelite.GetComponent<Rigidbody>().mass;
    }

    public void DoControl()
    {
        Vector3 direction = (Planet.transform.position - Satelite.transform.position).normalized;

        r = Vector3.Distance(Satelite.transform.position, Planet.transform.position);
        f = (G * m * M * direction) / (r * r);
        SateliterRig.AddForce(f, ForceMode.Impulse);
    }


    public RoboPartsConfigData[] GetRoboPartsConfig()
    {
        return null;
    }
    public RosTopicMessageConfig[] getRosConfig()
    {
        return null;
    }
}
