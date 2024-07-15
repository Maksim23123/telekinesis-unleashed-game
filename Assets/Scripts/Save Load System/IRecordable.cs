using System;

public interface IRecordable
{
    public virtual int Priority { get => 0; }

    event Action RunOutOfHealth;

    public abstract ObjectData GetObjectData();

    public abstract void SetObjectData(ObjectData objectData);
}
