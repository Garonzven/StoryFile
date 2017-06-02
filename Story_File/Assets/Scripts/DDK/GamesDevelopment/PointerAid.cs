using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace DDK.GamesDevelopment {

	public class PointerAid : MonoBehaviour {

	    [System.Serializable]
	    public class PointerTargets
	    {
	        [Tooltip("Transform for the pointer to go. It can be in Screen Space or World Space")]
	        public Transform target;
	        [Tooltip("Indicates if there is a click animation when it reachs the target")]
	        public bool isEndClick = false;
	        [Tooltip("Indicates if the the oject attached will be active")]
	        public bool isObjectAttachedActive = false;
	    }

	    [Header("Pointer Attributes")]
	    [Tooltip("Pointer Object with the image attached")]
	    public GameObject pointer;
	    [Tooltip("The number of targets the pointer will move in sequence")]
	    public PointerTargets[] targets;

	    [Header("Object Attached Attributes")]
	    [Tooltip("An onject to follow the cursor if needed")]
	    public GameObject objectAttached = null;
	    [Tooltip("If the object attached is in 3D space, in wich depth is going to follow the pointer")]
	    public float depth;

	    [Header("Movement Attributes")]
	    [Tooltip("Speed the pointer will hace when moving")]
	    public float speed;
	    [Tooltip("Error tolerance for the distance of the pointer and the current target")]
	    public float threshold;

	    [Header("Click animation Attributes")]
	    [Tooltip("Bigger scale whent clcik animation happends")]
	    public float scaleWhenClick = 1.25f;
	    [Tooltip("Bigger scale whent clcik animation happends")]
	    public float clickLenght = 1f;


	    // initial position of the cursor
	    private Vector3 _initPos { get; set; }
	    // current target position vector
	    private Vector3 _targetPos;
	    // indicates if the moving is in course


	    void Start()
	    {
	        UpdateAttachedObject(objectAttached);
	        _initPos = pointer.transform.position;
	        objectAttached = new GameObject();
	    }

	    // Start the animation moves
	    public void StartMove() {
	        StartCoroutine(Move());
		}

	    /*
	     * Updates only the targets of each pointerTarget, given an array of transform.
	     */
	    public bool UpdateTargets(Transform[] newTargets)
	    {
	        int i = 0;
	        if (newTargets.Length == targets.Length)
	        {
	            foreach (Transform t in newTargets)
	            {
	                targets[i].target = t;
	                i++;
	            }

	            return true;
	        }
	        return false;
	    }


	    /*
	     * Updates only the targets of each pointerTarget, given a list of transform.
	     */
	    public void UpdateTargets(List<Transform> newTargets)
	    {
	        int i = 0;
	        if (newTargets.Count == targets.Length)
	        {
	            foreach (Transform t in newTargets.ToArray())
	            {
	                targets[i].target = t;
	                i++;
	            }
	        }
	    }

	    /*
	     * Updates the attached object given a new GameObject.
	     */
	    public void UpdateAttachedObject(GameObject newObject)
	    {
	        if (objectAttached)
	        {
	            Destroy(objectAttached);
	        }  
	        objectAttached = newObject;
	    }

	    /*
	     * Updates only the targets of each pointerTarget, given an array of transform.
	     */
	    public void UpdateInitPosition(Transform newObject)
	    {
	        _initPos = newObject.position;
	    }

	    void Update()
	    {
	        if (Input.anyKeyDown)
	        {
	            StopCoroutine(Move());
	            pointer.gameObject.SetActive(false);
	            if (objectAttached)
	            {
	                objectAttached.gameObject.SetActive(false);
	            }
	        }
	    }

	    /*
	     * Coroutine to make the movement of the pointer and th click animation
	     */
	    IEnumerator Move()
	    {
	        pointer.transform.position = _initPos;
	        Vector3 dir;
	        pointer.gameObject.SetActive(true);

	        // Move to each target
	        for (int i = 0; i < targets.Length; i++)
	        {
	            //If activate object attached, if requested.
	            if (objectAttached)
	            {
	                if (targets[i].isObjectAttachedActive)
	                {
	                    objectAttached.gameObject.SetActive(true);
	                }
	                else
	                {
	                    objectAttached.gameObject.SetActive(false);
	                }
	                
	            }
	            if (targets[i].target.gameObject.GetComponent<RectTransform>())
	            {
	                _targetPos = targets[i].target.position;
	            }
	            else
	            {
	                _targetPos = Camera.main.WorldToScreenPoint(targets[i].target.position);
	            }
	            
	            // Move Pointer to Target
	            while (Vector3.Distance(pointer.transform.position,_targetPos) > threshold)
	            {
	                yield return new WaitForSeconds(0.001f);

	                dir = Vector3.Normalize(_targetPos - pointer.transform.position);
	                pointer.transform.position += dir * speed;

	                // Move object attached with pointer
	                if (objectAttached)
	                {
	                    if (objectAttached.GetComponent<RectTransform>())
	                    {
	                        objectAttached.transform.position = pointer.transform.position;
	                    }
	                    else
	                    {
	                        objectAttached.transform.position = Camera.main.ScreenToWorldPoint(
	                            new Vector3(pointer.transform.position.x, pointer.transform.position.y, depth)
	                        );
	                    }
	                }
	            }

	            // Make a click animation if requested
	            if (targets[i].isEndClick)
	            {
	                float size;
	                for (int j = 1; j <= 10; j++)
	                {
	                    size = Mathf.Lerp(1, scaleWhenClick, j / 10);
	                    pointer.transform.localScale = new Vector3(size, size, size);
	                    yield return new WaitForSeconds(clickLenght/20);
	                }

	                for (int j = 10; j > 0; j--)
	                {
	                    size = Mathf.Lerp(scaleWhenClick, 1, j / 10);
	                    pointer.transform.localScale = new Vector3(size, size, size);
	                    yield return new WaitForSeconds(clickLenght / 20);
	                }
	            }
	        }

	        //Deactivate pointer and object attached at the end of the animation.
	        pointer.gameObject.SetActive(false);
	        if (objectAttached)
	        {
	            objectAttached.gameObject.SetActive(false);
	        }

	    }

	}
}