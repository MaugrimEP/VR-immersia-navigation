using UnityEngine;

using System.Collections;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

/// <summary>
/// Change character height according to the head position.
/// </summary>
public class AutoHeightNavigation : MonoBehaviour
{
	CharacterController character;
    NavMeshAgent navMeshAgent;

	/// <summary>
	/// Tracked head user.
	/// </summary>
	public GameObject head;

	/// <summary>
	/// Minimal character controller height.  
	/// </summary>
    public Vector2 minMaxHeight = new Vector2(0.4f, 1);

	/// <summary>
	/// If present, correctly display the character controller renderer. 
	/// </summary>
	public Renderer characterRenderer;

	void Start ()
	{
		character = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
	}

	void Update ()
	{
		float currentHeight = head.transform.position.y;

        character.height = Mathf.Clamp(currentHeight, minMaxHeight[0], minMaxHeight[1]);
        character.center = new Vector3(0, character.height / 2.0f + character.skinWidth, 0);

        navMeshAgent.height = character.height;

		if(characterRenderer != null)
		{
			characterRenderer.transform.localScale = new Vector3(characterRenderer.transform.localScale.x,
			                                                     character.center.y, 
			                                                     characterRenderer.transform.localScale.z);

            characterRenderer.transform.localPosition = new Vector3(0, character.height / 2, 0);
		}
	}
}
