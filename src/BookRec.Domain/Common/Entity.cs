 /// <summary>
 /// Allows all other model classes to inherit from from Entity class. Removes redundancy.
 /// </summary>
 public abstract class Entity
{
    /// <summary>
    /// Unique id for all Entities. Only the Entity class can modify the id.
    /// </summary>
    /// <value>Entity Identifier</value>
    public Guid Id { get; set; }

    public Entity(Guid id)
    {
        Id = id;
    }


}