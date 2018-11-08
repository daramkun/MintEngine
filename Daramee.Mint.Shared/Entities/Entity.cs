using Daramee.Mint.Collections;
using Daramee.Mint.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Daramee.Mint.Entities
{
	public class Entity
	{
		internal ObservableCollection<IComponent> components;
		
		public Guid UniqueId { get; internal set; }
		public string Name { get; set; }
		public string Tag { get; set; }

		public bool IsActived { get; set; }

		public event EventHandler EntityDestroyed;
		public event EventHandler<IComponent> ComponentAdded, ComponentRemoved;

		internal Entity () { Initialize (); }

		~Entity ()
		{
			EntityManager.SharedManager?.DestroyEntity ( this );
		}
		internal void Destroyed () { EntityDestroyed?.Invoke ( this, EventArgs.Empty ); }

		internal void Initialize ()
		{
			UniqueId = Guid.NewGuid ();
			Name = "New Entity";
			Tag = "";

			IsActived = true;

			if ( components == null )
				components = new ObservableCollection<IComponent> ();
		}

		public IComponent AddComponent ( Type type )
		{
			if ( HasComponent ( type ) )
				return null;
			
			IComponent component = EntityManager.SharedManager.CreateComponent ( type );
			components.Add ( component );
			EntityManager.SharedManager.RegisterComponent ( this, component );

			ComponentAdded?.Invoke ( this, component );

			return component;
		}
		public T AddComponent<T> () where T : IComponent
		{
			return ( T ) AddComponent ( typeof ( T ) );
		}

		public void RemoveComponent ( IComponent component )
		{
			components.Remove ( component );
			EntityManager.SharedManager.UnregisterComponent ( this, component );

			ComponentRemoved?.Invoke ( this, component );
		}
		public void RemoveComponent<T> () where T : IComponent
		{
			RemoveComponent ( GetComponent<T> () );
		}

		public IComponent GetComponent ( Type type )
		{
			foreach ( var component in components )
				if ( component.GetType () == type )
					return component;
			return null;
		}
		public T GetComponent<T> () where T : IComponent
		{
			return ( T ) GetComponent ( typeof ( T ) );
		}

		public bool HasComponent ( Type type )
		{
			foreach ( var component in components )
				if ( component.GetType () == type )
					return true;
			return false;
		}
		public bool HasComponent<T> () where T : IComponent
		{
			return HasComponent ( typeof ( T ) );
		}

		public int ComponentCount => components.Count;

		public void ClearComponents ()
		{
			foreach ( var component in components )
				ComponentRemoved?.Invoke ( this, component );
			components.Clear ();
		}

		public IEnumerable<IComponent> GetComponents ( Func<IComponent, bool> condition = null )
			=> new ForEachSafeEnumerable<IComponent> ( components, condition );

		public override string ToString () => $"{{Entity Id: {UniqueId}, Name: {Name}, Tag: {Tag}, Components: {components.Count}}}";
	}
}
