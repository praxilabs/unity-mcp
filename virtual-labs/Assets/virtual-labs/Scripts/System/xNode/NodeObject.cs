[System.Serializable]
public class NodeObject
{
    public object value;

    public NodeObject()
    {
        value = null;
    }

    public NodeObject(object value)
    {
        this.value = value;
    }
}