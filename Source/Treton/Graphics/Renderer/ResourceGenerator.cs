using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer
{
	[Serializable]
	public class ResourceGenerator : IDisposable
	{
		public uint Name;
		[NonSerialized]
		public Modifiers.IModifier[] Modifiers;
		public ModifierDefinition[] ModifierDescriptions;

		public ResourceGenerator()
		{

		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ResourceGenerator()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var modifier in Modifiers)
				{
					modifier.Dispose();
				}
			}
		}

		public void Initialize(Core.Resources.IResourceManager resourceManager)
		{
			Modifiers = new Modifiers.IModifier[ModifierDescriptions.Length];
			for (var i = 0; i < ModifierDescriptions.Length; i++)
			{
				var description = ModifierDescriptions[i];
				// TODO: This is crap, but so is the entire render pipeline at the moment
				if (description.Type == Core.Hash.HashString("FullscreenPass"))
				{
					var material = resourceManager.Get<Material>(description.Material);
					Modifiers[i] = new Modifiers.FullscreenPass(material);
				}
			}
		}

		public void Execute(RenderSystem renderSystem)
		{
			foreach (var modifier in Modifiers)
			{
				modifier.Execute(renderSystem);
			}
		}

		[Serializable]
		public class ModifierDefinition
		{
			public uint Type;
			public Core.Resources.ResourceId Material;
		}
	}
}
