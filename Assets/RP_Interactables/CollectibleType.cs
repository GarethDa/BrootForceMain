using UnityEngine;

public class CollectibleType : InteractiveType
{
    public CollectibleTypes thisType;
    public float collectionValue = 1f;

    [Tooltip("If enabled, this makes it so the collection value is multiplied by the scale when determining " +
        "the collection value of this object. E.g. a water droplet with a collectionValue of 2 and a scale range " +
        "of 0.75-2 could be worth anything between 1.5 and 4.")]
    public bool valueWithScale = false;

    public bool revealValue = false;

    private float thisValue;

    private void Update()
    {
        if (revealValue)
            Debug.Log(thisValue);
    }

    //Method for setting the value, takes a scale as input (but doesn't necessarily use it)
    public void SetThisValue(float scale)
    {
        //If the value is set to align with the scale, then multiply the value by the scale
        if (valueWithScale)
            thisValue = Mathf.Round(scale * collectionValue * 100f) / 100f;

        //Otherwise, the value should just be the normal specified value
        else
            thisValue = Mathf.Round(collectionValue * 100f) / 100f;
    }

    public float GetValue()
    { return thisValue; }

    public enum CollectibleTypes
    {
        water
    }
}
