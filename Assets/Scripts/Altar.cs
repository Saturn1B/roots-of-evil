using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Altar : MonoBehaviour
{
    [SerializeField] List<GameObject> corruptionZones = new List<GameObject>();
    [SerializeField] List<GameObject> corruptionZonesActive = new List<GameObject>();

    [SerializeField] List<MeshRenderer> pillars = new List<MeshRenderer>();
    [SerializeField] Material pillarOnMat;

    [SerializeField] ParticleSystem blood01;
    [SerializeField] ParticleSystem blood02;

    int sacrificeCount;
    [SerializeField] int maxSacrifice = 10;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Corrupt()
    {
        blood01.Play();
        blood02.Play();

        int r = Random.Range(0, corruptionZones.Count);

        corruptionZones[r].transform.position = 
            new Vector3(corruptionZones[r].transform.position.x, -2, corruptionZones[r].transform.position.z);

        corruptionZones[r].SetActive(true);

        corruptionZones[r].transform.DOMoveY(0f, 3f);

        corruptionZonesActive.Add(corruptionZones[r]);
        corruptionZones.Remove(corruptionZones[r]);

        sacrificeCount++;

        CheckCorruption();
    }

    void CheckCorruption()
    {
        if (sacrificeCount == maxSacrifice / 5)
        {
            pillars[0].material = pillarOnMat;
        }
        else if (sacrificeCount == 2 * (maxSacrifice / 5))
        {
            pillars[1].material = pillarOnMat;
        }
        else if (sacrificeCount == 3 * (maxSacrifice / 5))
        {
            pillars[2].material = pillarOnMat;
        }
        else if (sacrificeCount == 4 * (maxSacrifice / 5))
        {
            pillars[3].material = pillarOnMat;
        }
        else if(sacrificeCount == maxSacrifice)
        {
            pillars[4].material = pillarOnMat;
        }
    }

    public bool CheckAnimalTarget(Vector3 target)
    {
        foreach(GameObject zone in corruptionZonesActive)
        {
            Debug.Log("try");
            if (Vector3.Distance(zone.transform.position, target) <= 15)
            {
                Debug.Log("inf");
                return false;
            }
        }

        return true;
    }
}
