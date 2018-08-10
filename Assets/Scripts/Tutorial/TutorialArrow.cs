using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour {

    public Transform arrowTip;
    public bool flipArrow = false;

    [HideInInspector]
    public Transform trackTransform = null;

    private float angle = 0;

    public void PlaceArrow(Vector3 position, float _angle)
    {
        angle = _angle;
        Vector3 orignalRotation = transform.localEulerAngles;
        transform.rotation = Quaternion.AngleAxis(_angle, transform.forward);
        transform.localEulerAngles = new Vector3(orignalRotation.x, transform.localEulerAngles.y + (flipArrow ? 180 : 0), transform.localEulerAngles.z); //restricts rotation in only y
        transform.position = position + (transform.position - arrowTip.position);
        
    }

    public void PlaceArrow(Spawn spawn)
    {
        if (spawn.spawnPoint.GetComponent<RectTransform>() != null && GetComponent<RectTransform>() != null)
        {
            GetComponent<RectTransform>().anchorMax = spawn.spawnPoint.GetComponent<RectTransform>().anchorMax;
            GetComponent<RectTransform>().anchorMin = spawn.spawnPoint.GetComponent<RectTransform>().anchorMin;
        }
        PlaceArrow(spawn.spawnPoint.position, spawn.angle);
        
    }

    public void PlaceArrow(List<Spawn> spawns, string _name)
    {
        Spawn spawn = spawns.Find(x => x.spawnPoint.name == _name);
        if(spawn == null)
        {
            Debug.LogError("Could not find a Transform with that name!");
            DeleteArrow();
            return;
        }
        name = _name;
        PlaceArrow(spawn);
    }

    public void DeleteArrow()
    {
        Destroy(gameObject);
    }

    [System.Serializable]
    public class Spawn
    {
        public Transform spawnPoint;
        public float angle;
    }

    public void TrackTransform(Transform _trackTransform, float _angle)
    {
        trackTransform = _trackTransform;
        angle = _angle;
    }

    private void Update()
    {
        if(trackTransform != null)
        {
            PlaceArrow(trackTransform.position, angle);
        }
    }
}
