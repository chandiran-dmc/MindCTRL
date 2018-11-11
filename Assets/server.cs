using UnityEngine;
using System.Threading;
using SharpOSC;
using System.Collections;

// muse-player -l udp:7000 -s osc.udp://localhost:5000

public class server : MonoBehaviour {

    Thread thread;
    static float[] alpha;
    static float[] beta;
    public GameObject squarf;
    static ArrayList alphaAHistory, alphaDHistory;
    static float alphaA_MA, alphaD_MA;

    void Start () {
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();

        alpha = new float[4];
        beta = new float[4];
        alphaAHistory = new ArrayList();
        alphaDHistory = new ArrayList();
        alphaA_MA = alphaD_MA = -1;
	}
	
	// Update is called once per frame
	void Update () {
        alphaAHistory.Add(alpha[0]);
        alphaDHistory.Add(alpha[3]);
        if (alphaAHistory.Count > 70)
        {
            alphaAHistory.RemoveAt(0);
            alphaA_MA = Average(alphaAHistory);
        }
        if (alphaDHistory.Count > 70)
        {
            alphaDHistory.RemoveAt(0);
            alphaD_MA = Average(alphaDHistory);
        }
        if (alphaA_MA > -1 && alphaD_MA > -1)
        {
            Debug.Log(alphaA_MA + alphaD_MA);
            squarf.GetComponent<player>().setFactor(alphaA_MA + alphaD_MA);
        }
    }

    private float Average(ArrayList list)
    {
        float sum = 0;
        foreach (float f in list)
        {
            sum += f;
        }
        return sum / list.Count;
    }

    private void ThreadMethod()
    {
        HandleOscPacket callback = delegate (OscPacket packet)
        {
            var messageReceived = (OscMessage)packet;
            var addr = messageReceived.Address;
            if (addr == "Person0/elements/alpha_relative")
            {
                for (int i = 0; i < messageReceived.Arguments.Count; i++)
                {
                    alpha[i] = (float)messageReceived.Arguments[i];
                }
            }
            else if (addr == "Person0/elements/beta_relative")
            {
                for (int i = 0; i < messageReceived.Arguments.Count; i++)
                {
                    beta[i] = (float)messageReceived.Arguments[i];
                }
            }
        };

        var listener = new UDPListener(5000, callback);
        // listener.Close();
    }
}
