﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Treton.Core.Resources;
using Treton.Graphics;
using Treton.Core.IO;
using Treton.Graphics.Renderer;
using Treton.Core;
using Treton.Graphics.World;

namespace Treton.Framework
{
	public class Engine : IDisposable
	{
		// Threading
		private readonly Core.MainThreadScheduler _mainThreadScheduler;
		private readonly Stopwatch _timer;

		// Configuration
		private readonly EngineConfiguration _configuration;

		// IO
		private readonly IFileSystem _fileSystem;

		// Resources
		private readonly IResourceLoaders _resourceLoaders;
		private readonly IResourceManager _resourceManager;
		private readonly IPackage _coreResources;

		// Windowing & OpenGL
		private OpenTK.INativeWindow _window;
		private GraphicsContext _context;
		private DebugProc _debugProcCallback;
		private RenderSystem _renderSystem;
		private Renderer _renderer;

		public Engine(EngineConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException("configuration");

			_configuration = configuration;

			// Init threading framework
			_mainThreadScheduler = new Core.MainThreadScheduler(Thread.CurrentThread);
			_timer = new Stopwatch();

			// IO setup
			_fileSystem = FileSystem.CreateDefault(_configuration.DataDirectory);

			// Setup resource manager
			_resourceLoaders = new ResourceLoaders();
			_resourceLoaders.Add(Core.Hash.HashString("material"), new Graphics.ResourceLoaders.MaterialLoader(_mainThreadScheduler));
			_resourceLoaders.Add(Core.Hash.HashString("renderconfig"), new Graphics.ResourceLoaders.RenderConfigLoader(_mainThreadScheduler));
			_resourceManager = new ResourceManager(_resourceLoaders);

			// Core resources
			_coreResources = OpenPackage(_configuration.CorePackage);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Engine()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_renderSystem != null)
				{
					_renderSystem.Dispose();
					_renderSystem = null;
				}

				if (_context != null)
				{
					_context.Dispose();
					_context = null;
				}

				if (_window != null)
				{
					_window.Dispose();
					_window = null;
				}
			}
		}

		public IPackage OpenPackage(string name)
		{
			var path = _fileSystem.GetDataPath(name + ".package");
			return new Package(_resourceManager, path);
		}

		private void Initialize()
		{
			// Create window
			_window = new NativeWindow(_configuration.Renderer.Width, _configuration.Renderer.Height, _configuration.Title, GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default);
			_window.Visible = true;

			// Setup gl context + render system
			_context = new GraphicsContext(GraphicsMode.Default, _window.WindowInfo, 4, 4, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug);
			_context.MakeCurrent(_window.WindowInfo);
			_context.LoadAll();

			_debugProcCallback = GLDebugCallback;
			GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
			GL.Enable(EnableCap.DebugOutput);

			_renderSystem = new RenderSystem();

			// Load core package
			var coreResourcesLoadTask = _coreResources.Load();

			while (!coreResourcesLoadTask.IsCompleted)
			{
				_mainThreadScheduler.Tick(_timer, 10000);
			}

			// Setup renderer
			var renderConfig = _resourceManager.Get<RendererConfiguration>(new ResourceId(_configuration.Renderer.RenderConfig, "renderconfig"));
			_renderer = new Renderer(_renderSystem, renderConfig);
		}

		private void GLDebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
		{
			var msg = Marshal.PtrToStringAnsi(message, length);
			Console.WriteLine(string.Format("{0} {1} {2}", severity, type, msg));
		}

		private void TearDown()
		{
			_coreResources.Unload();

			// Run gc pass on the resource manager (if anything is not unloaded now then the calling application is at fault)
			var resourceManagerCleanUpTask = _resourceManager.GarbageCollect();

			while (!resourceManagerCleanUpTask.IsCompleted)
			{
				_mainThreadScheduler.Tick(_timer, 10000);
			}
		}

		/// <summary>
		/// Run the main loop
		/// </summary>
		public void Run(IApplication application)
		{
			if (application == null)
				throw new ArgumentNullException("application");

			Initialize();
			application.Initialize();

			var material = _resourceManager.Get<Material>(new ResourceId("Core/Materials/Test", "material"));
			var mesh = new Mesh(new VertexFormat(new VertexFormatElement[0]), new byte[0], new byte[0], new Mesh.SubMesh[] { new Mesh.SubMesh(0, 3) }, new Material[] { material });

			var renderWorld = new RenderWorld();
			renderWorld.AddMesh(mesh);

			var running = true;
			while (running)
			{
				// Tick background tasks that ""must"" run on the main thread
				_mainThreadScheduler.Tick(_timer, 500);

				application.Update(0.0);

				// Render
				GL.ClearColor(Color4.AliceBlue);
				GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

				_renderer.Render(renderWorld, null, null);

				_context.SwapBuffers();

				// Process window events
				_window.ProcessEvents();

				running = _window.Exists;
			}

			mesh.Dispose();

			application.Shutdown();
			TearDown();
		}
	}
}