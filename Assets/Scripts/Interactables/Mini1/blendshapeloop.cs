using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeController : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    int blendShapeCount;

    int playIndex = 0;

    // Variables for specific action sequences
    public int actionStartIndex = 0;
    public int actionEndIndex = 0;
    public bool isActionActive = false;
    public float actionDuration = 1.0f; // Duration of the action sequence
    private float actionTimer = 0.0f;

    private void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        blendShapeCount = skinnedMesh.blendShapeCount;
    }

    private void Update()
    {
        if (isActionActive)
        {
            // Handle the action sequence
            actionTimer += Time.deltaTime;
            float actionProgress = Mathf.Clamp01(actionTimer / actionDuration);
            int actionBlendShapeIndex = Mathf.RoundToInt(Mathf.Lerp(actionStartIndex, actionEndIndex, actionProgress));

            // Reset all blend shapes to 0 before setting the active one
            for (int i = 0; i < blendShapeCount; i++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
            }

            if (actionBlendShapeIndex < blendShapeCount)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(actionBlendShapeIndex, 100f);
            }

            if (actionTimer >= actionDuration)
            {
                // Stop the action without resetting isActionActive
                actionTimer = 0.0f;
                // Optionally set blend shape weights to 0 or maintain the last frame
                skinnedMeshRenderer.SetBlendShapeWeight(actionEndIndex, 0f);
                // Optionally, you can call a method or set a flag to indicate completion
                // e.g., OnActionComplete(); or SetActionCompleteFlag();
            }
        }
        else
        {
            // Handle the smooth looping animation
            for (int i = 0; i < blendShapeCount; i++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
            }

            if (playIndex > 0) skinnedMeshRenderer.SetBlendShapeWeight(playIndex - 1, 0f);
            if (playIndex == 0) skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 0f);

            skinnedMeshRenderer.SetBlendShapeWeight(playIndex, 100f);
            playIndex++;
            if (playIndex > blendShapeCount - 1) playIndex = 0;
        }
    }

    // Method to start an action sequence
    public void StartAction(int startIndex, int endIndex, float duration)
    {
        actionStartIndex = startIndex;
        actionEndIndex = endIndex;
        actionDuration = duration;
        actionTimer = 0.0f;
        isActionActive = true;
    }

    // Optionally, method to stop the action manually
    public void StopAction()
    {
        isActionActive = false;
        actionTimer = 0.0f;
        // Optionally reset blend shape weights or take other actions
        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
        }
    }
}