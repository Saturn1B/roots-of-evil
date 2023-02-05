using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Altar : MonoBehaviour
{
    [SerializeField] List<GameObject> corruptionZones = new List<GameObject>();
    [SerializeField] List<GameObject> corruptionZonesActive = new List<GameObject>();

    [SerializeField] List<MeshRenderer> pillars = new List<MeshRenderer>();
    [SerializeField] Material pillarOnMat;

    [SerializeField] ParticleSystem blood01;
    [SerializeField] ParticleSystem blood02;
    [SerializeField] ParticleSystem beaconEffect;

    int sacrificeCount;
    [SerializeField] int maxSacrifice = 10;

    [SerializeField] AudioClip sacrifice;
    [SerializeField] AudioClip beacon;
    [SerializeField] AudioClip music;

    [SerializeField] GameObject endDialog;
    public bool end;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayMusic(music);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        BackToMenu();
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
            SoundManager.Instance.Play(sacrifice);
        }
        else if (sacrificeCount == 2 * (maxSacrifice / 5))
        {
            pillars[1].material = pillarOnMat;
            SoundManager.Instance.Play(sacrifice);
        }
        else if (sacrificeCount == 3 * (maxSacrifice / 5))
        {
            pillars[2].material = pillarOnMat;
            SoundManager.Instance.Play(sacrifice);
        }
        else if (sacrificeCount == 4 * (maxSacrifice / 5))
        {
            pillars[3].material = pillarOnMat;
            SoundManager.Instance.Play(sacrifice);
        }
        else if(sacrificeCount == maxSacrifice)
        {
            pillars[4].material = pillarOnMat;
            SoundManager.Instance.Play(beacon);
            beaconEffect.Play();
            endDialog.SetActive(true);
            StartCoroutine(End());
        }
    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(5.1f);
        end = true;
    }

    void BackToMenu()
    {
        if (end && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Menu");
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
