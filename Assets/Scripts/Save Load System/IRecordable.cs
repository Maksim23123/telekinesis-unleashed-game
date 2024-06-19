public interface IRecordable
{
    public virtual int Priority { get => 0; }

    public abstract ObjectData GetObjectData();

    public abstract void SetObjectData(ObjectData objectData);
}
