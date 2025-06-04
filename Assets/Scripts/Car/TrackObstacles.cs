using Mirror;
using UnityEngine;

public class TrackObstacles : NetworkBehaviour
{
    private GameObject _barrels;

    private void Start()
    {
        GetBarrels();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Barrel"))
        {
            CmdDestroyBarrel(other.gameObject.GetComponent<Barrel>().barrelID);
        }
    }

    [Command]
    private void CmdDestroyBarrel(string barrelID)
    {
        RpcDestroyBarrel(barrelID);
    }

    [ClientRpc]
    private void RpcDestroyBarrel(string barrelID)
    {
        for (int i = 0; i < _barrels.transform.childCount; i++)
        {
            if (_barrels.transform.GetChild(i).gameObject.GetComponentInChildren<Barrel>().barrelID == barrelID)
            {
                _barrels.transform.GetChild(i).gameObject.GetComponentInChildren<Barrel>().DestroyBarrel();
                break;
            }
        }
    }
    
    private void GetBarrels()
    {
        if (_barrels == null)
        {
            _barrels = GameObject.FindGameObjectWithTag("Barrels");
        }
    }
}
