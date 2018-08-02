using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Defender : NetworkBehaviour {

    [SerializeField]
    public Building[] buildings;

	[Command]
    public void CmdSpawnBuilding(string buildingName)
    {

        Building building = System.Array.Find(buildings, x => x.name == buildingName);
        Building newBuilding = Instantiate(building);
        NetworkServer.SpawnWithClientAuthority(newBuilding.gameObject, connectionToClient);

    }

}
