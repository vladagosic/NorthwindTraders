namespace Northwind.Domain.Entities
{
	public abstract class Entity
	{
		// The Id will be set automatically by the Entity Framework.
		// That's why we need the "private set".
		public int Id { get; private set; }
	}
}
