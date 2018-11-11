using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpOSC;

public class gameController : MonoBehaviour {

    private const int NUM_TREES = 2 * 25; // total number of trees to be spawned
    private const int SPAWN_AREA = 50;

    public GameObject player;
    public GameObject cub;
    public GameObject zombie, crawler;
    public AudioSource spoopyNoise;
    public AudioSource zombieNoise1, zombieNoise2, zombieNoise3, zombieNoise4;
    public GameObject meanieBlock, bridge;
    private ArrayList zombies = new ArrayList();
    private Vector3 zombieMove;
    private float zed = 5.0f;
    private float offset = 0.0f;
    private ArrayList blockos = new ArrayList();
    private GameObject[] treesLeft, treesRight;
    public GameObject tree1, tree2, tree3, tree4, bush1, bush2, bush3, bush4, bush5, bush6;
    private GameObject[] TREE_PREFABS;

    // Use this for initialization
	void Start () {
        spawnCube();
        spawnCube();
        spawnCube();
        spawnCube();
        spawnCube();
        treesLeft = new GameObject[NUM_TREES / 2];
        treesRight = new GameObject[NUM_TREES / 2];
        TREE_PREFABS = new GameObject[] { tree1, tree2, tree3, tree4, bush1, bush2, bush3, bush4, bush5, bush6 };
        for (int i = 0; i < NUM_TREES; i++)
        {
            GameObject newTree = Instantiate(TREE_PREFABS[Mathf.FloorToInt(Random.value * TREE_PREFABS.Length)]) as GameObject;
            newTree.transform.SetParent(transform);
            float x = Random.Range(-0.5f, 0.5f) + ((i % 2 == 0) ? -4 : 4);
            float y = -0.5f;
            float z = -SPAWN_AREA / 2 + ((float) i / NUM_TREES) * SPAWN_AREA;
            newTree.transform.position = new Vector3(x, y, z);
            if (i % 2 == 0)
            {
                treesLeft[i / 2] = newTree;
            }
            else
            {
                treesRight[i / 2] = newTree;
            }
        }
    }

    void spawnCube()
    {
        // make the thing do
        GameObject go;
        if (Random.Range(0.0f, 1.0f) > 0.2f)
        {
            go = Instantiate(cub) as GameObject;
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(offset, 0, zed);//Vector3.forward * zed;
            zed += 5.0f;
            if (Random.Range(0.0f, 1.0f) > .2f)
            {
                spawnObstacle();
            }
        }
        else
        {
            if (zed < 70.0f)
            {
                go = Instantiate(cub) as GameObject;
                go.transform.SetParent(transform);
                go.transform.position = new Vector3(offset, 0, zed);
                zed += 5.0f;
            }
            else if (player.GetComponent<CharacterController>().velocity.z < 14.0f)
            {
                go = Instantiate(bridge) as GameObject;
                go.transform.SetParent(transform);
                offset += Random.Range(-1.75f, 1.75f);
                go.transform.position = new Vector3(offset, 0, zed);
                offset += Random.Range(-1.75f, 1.75f);
                zed += 5.0f;
            }
            else
            {
                //go = Instantiate(meanieBlock) as GameObject;
                go = Instantiate(cub) as GameObject;
                spoopyNoise.Play();
                go.transform.SetParent(transform);
                offset += Random.Range(-2.5f,2.5f);
                go.transform.position = new Vector3(offset, 0, zed+5.0f);
                zed += 10.0f;
            }
        }
        blockos.Add(go);
    }

    void startServer()
    {
        Debug.Log("cs1\n\n");

        HandleOscPacket callback = delegate (OscPacket packet)
        {
            var messageReceived = (OscMessage)packet;
            var addr = messageReceived.Address;
            if (addr == "/muse/eeg")
            {
                Debug.Log("EEG values: ");
                foreach (var arg in messageReceived.Arguments)
                {
                    Debug.Log(arg + " ");
                }
            }
            if (addr == "/muse/elements/alpha_relative")
            {
                Debug.Log("Relative Alpha power values: ");
                foreach (var arg in messageReceived.Arguments)
                {
                    Debug.Log(arg + " ");
                }
            }
        };
        Debug.Log("cs2\n\n");

        // Create an OSC server.
        var listener = new UDPListener(5000, callback);
        Debug.Log("cs2\n\n");

    }

    void spawnObstacle()
    {
        GameObject go;
        float rand = Random.value;
        if (rand < 0.4)
        {
            go = Instantiate(zombie) as GameObject;
            zombies.Add(go);
            go.transform.position = new Vector3(0, go.transform.position.y, 0);
            if (rand < 0.1)
            {
                zombieNoise1.Play();
            }
            else if (rand < 0.2)
            {
                zombieNoise2.Play();
            }
        }
        else
        {
            go = Instantiate(crawler) as GameObject;
            zombies.Add(go);
            go.transform.position = new Vector3(0, go.transform.position.y-0.5f,0);
            if (rand < 0.5)
            {
                zombieNoise3.Play();
            }
            else if (rand < 0.6)
            {
                zombieNoise4.Play();
            }
        }
        go.transform.SetParent(transform);
        Vector3 dink = new Vector3(offset, go.transform.position.y, zed - 5.0f);//Vector3.forward * (zed-5.0f);
        dink.x = Random.Range(offset-2.0f, offset+2.0f);
        dink.y = .5f;
        zombieMove = Vector3.forward * -0.05f;
        go.transform.position = dink;
    }
	public void removeZombie(GameObject zom)
    {
        zombies.Remove(zom);
        Destroy(zom);
    }
	// Update is called once per frame
	void Update () {
        foreach (GameObject go in blockos)
        {
            if (player.transform.position.z - (SPAWN_AREA / 2) > go.transform.position.z)
            {
                blockos.Remove(go);
                Destroy(go);
                break;
            }
        }
        if (blockos.Count < SPAWN_AREA / 5)
        {
            spawnCube();
        }
        Debug.Log("Length " + zombies.Count);
        for (int i=zombies.Count-1; i>=0; i--)
        {
            // still is null?
            if (zombies[i] != null)
                ((GameObject)zombies[i]).GetComponent<CharacterController>().Move(zombieMove);
        }

        foreach (GameObject tree in treesLeft)
        {
            if (player.transform.position.z - SPAWN_AREA / 2 > tree.transform.position.z)
            {
                tree.transform.position = new Vector3(offset - 4 + Random.Range(-0.5f, 0f), tree.transform.position.y, tree.transform.position.z + SPAWN_AREA);
            }
        }
        foreach (GameObject tree in treesRight)
        {
            if (player.transform.position.z - SPAWN_AREA / 2 > tree.transform.position.z)
            {
                tree.transform.position = new Vector3(offset + 4 + Random.Range(0f, 0.5f), tree.transform.position.y, tree.transform.position.z + SPAWN_AREA);
            }
        }
    }
}
