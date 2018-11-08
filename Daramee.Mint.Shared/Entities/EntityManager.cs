using Daramee.Mint.Collections;
using Daramee.Mint.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Daramee.Mint.Entities
{
	public sealed class EntityManager : IDisposable
	{
		public static EntityManager SharedManager { get; private set; }

		ObservableCollection<Entity> entities = new ObservableCollection<Entity> ();
		ConcurrentDictionary<Guid, Entity> entityDictionary = new ConcurrentDictionary<Guid, Entity> ();
		ConcurrentDictionary<IComponent, Entity> componentEntityRelation = new ConcurrentDictionary<IComponent, Entity> ();

		ConcurrentQueue<Entity> cachedEntities = new ConcurrentQueue<Entity> ();
		ConcurrentDictionary<Type, ConcurrentQueue<IComponent>> cachedComponents = new ConcurrentDictionary<Type, ConcurrentQueue<IComponent>> ();

		public IEnumerable<Entity> Entities => new ForEachSafeEnumerable<Entity> ( entities, null );

		internal EntityManager ()
		{
			if ( SharedManager != null )
				throw new InvalidOperationException ();

			SharedManager = this;
		}
		~EntityManager ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
			ClearEntities ();
			foreach ( var entity in cachedEntities )
				GC.SuppressFinalize ( entity );

			entities = null;
			entityDictionary = null;
			componentEntityRelation = null;
			cachedEntities = null;
			cachedComponents = null;

			SharedManager = null;

			GC.SuppressFinalize ( this );
		}

		public Entity CreateEntity ()
		{
			Entity entity;
			if ( !cachedEntities.IsEmpty )
			{
				cachedEntities.TryDequeue ( out entity );
				entity.Initialize ();
			}
			else entity = new Entity ();

			entities.Add ( entity );
			entityDictionary.TryAdd ( entity.UniqueId, entity );

			return entity;
		}
		public Entity CreateEntity ( Entity prefab )
		{
			if ( prefab == null )
				throw new ArgumentNullException ();

			Entity entity = CreateEntity ();

			entity.UniqueId = Guid.NewGuid ();
			entity.Name = prefab.Name;
			entity.Tag = prefab.Tag;

			entity.IsActived = prefab.IsActived;

			foreach ( var component in prefab.components )
				entity.AddComponent ( component.GetType () ).CopyFrom ( component );

			return entity;
		}

		public void DestroyEntity ( Entity entity )
		{
			if ( entity == null ) return;
			if ( entities.Contains ( entity ) )
			{
				entities.Remove ( entity );
				entityDictionary.TryRemove ( entity.UniqueId, out Entity temp );
			}
			entity.Destroyed ();
			foreach ( IComponent component in entity.components )
			{
				if ( !cachedComponents.ContainsKey ( component.GetType () ) )
					cachedComponents.TryAdd ( component.GetType (), new ConcurrentQueue<IComponent> () );
				cachedComponents [ component.GetType () ].Enqueue ( component );
			}
			entity.components.Clear ();
			cachedEntities.Enqueue ( entity );
		}

		public IComponent CreateComponent ( Type type )
		{
			if ( type == null )
				throw new ArgumentNullException ();

			IComponent component;
			if ( cachedComponents.ContainsKey ( type ) )
			{
				if ( cachedComponents [ type ].TryDequeue ( out component ) )
				{
					component.Initialize ();
					return component;
				}
			}
			component = Activator.CreateInstance ( type ) as IComponent;
			if ( component == null )
				throw new ArgumentException ();
			component.Initialize ();
			return component;
		}
		public T CreateComponent<T> () where T : IComponent
		{
			return ( T ) CreateComponent ( typeof ( T ) );
		}

		public IEnumerable<Entity> GetEntitiesByName ( string name )
			=> Entities.Where ( ( entity ) => entity.Name == name );
		public IEnumerable<Entity> GetEntitiesByComponent ( Type type )
			=> Entities.Where ( ( entity ) => entity.HasComponent ( type ) );
		public IEnumerable<Entity> GetEntitiesByComponent<T> ()
			=> GetEntitiesByComponent ( typeof ( T ) );
		public Entity GetEntityById ( Guid id )
		{
			if ( entityDictionary.ContainsKey ( id ) )
				return entityDictionary [ id ];
			return null;
		}
		public Entity GetEntityByComponent ( IComponent component )
		{
			return componentEntityRelation.ContainsKey ( component )
				? componentEntityRelation [ component ]
				: null;
		}

		public void ClearEntities ()
		{
			foreach ( var entity in Entities )
			{
				entity.Destroyed ();
				entity.ClearComponents ();
			}
			entities.Clear ();
			entityDictionary.Clear ();
			componentEntityRelation.Clear ();
		}

		public void AttachEntities ( IEnumerable<Entity> entities )
		{
			if ( this.entities == entities )
				throw new ArgumentException ();

			foreach ( Entity entity in entities )
			{
				if ( this.entities.Contains ( entity ) )
					continue;
				this.entities.Add ( entity );
				entityDictionary.TryAdd ( entity.UniqueId, entity );
				foreach ( IComponent component in entity.components )
					componentEntityRelation.TryAdd ( component, entity );
			}
		}
		public void DetachEntities ( ICollection<Entity> collection )
		{
			foreach ( var entity in entities )
				collection.Add ( entity );
			entityDictionary.Clear ();
			entities.Clear ();
			componentEntityRelation.Clear ();
		}

		internal bool RegisterComponent ( Entity entity, IComponent component )
		{
			if ( !entityDictionary.ContainsKey ( entity.UniqueId ) )
				return false;
			if ( componentEntityRelation.ContainsKey ( component ) )
				return false;
			return componentEntityRelation.TryAdd ( component, entity );
		}
		internal bool UnregisterComponent ( Entity entity, IComponent component )
		{
			if ( !entityDictionary.ContainsKey ( entity.UniqueId ) )
				return false;
			if ( !componentEntityRelation.ContainsKey ( component ) )
				return false;
			return componentEntityRelation.TryRemove ( component, out Entity temp );
		}
	}
}
