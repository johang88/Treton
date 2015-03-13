using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Treton.Core.Resources
{
	public interface IPackage
	{
		Task Load();
		void Unload();
		Stream Open(ResourceId resourceId);
	}

	public class Package : IPackage
	{
		private readonly IResourceManager _resourceManager;
		private readonly string _path;
		private readonly string _packagePath;
		private readonly ResourceId[] _resources;

		private readonly bool _isBundle;
		private readonly Dictionary<ResourceId, ResourceInfo> _resourceOffset = new Dictionary<ResourceId,ResourceInfo>();

		public Package(IResourceManager resourceManager, string path)
		{
			if (resourceManager == null)
				throw new ArgumentNullException("resourceManager");
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException("path");

			_resourceManager = resourceManager;
			_packagePath = path;
			_path = Path.GetDirectoryName(path);

			using (var stream = File.OpenRead(_packagePath))
			using (var reader = new BinaryReader(stream))
			{
				var magic = new string(reader.ReadChars(4));
				if (magic != "TPAK")
					throw new InvalidOperationException("invalid package");

				_isBundle = reader.ReadBoolean();

				var numEntries = reader.ReadInt32();
				_resources = new ResourceId[numEntries];

				// Read entries
				for (var i = 0; i < numEntries; i++)
				{
					_resources[i] = new ResourceId(
						reader.ReadUInt32(), reader.ReadUInt32()
						);
				}

				// Read bundle info
				if (_isBundle)
				{
					for (var i = 0; i < numEntries; i++)
					{
						_resourceOffset.Add(_resources[i], new ResourceInfo
						{
							Length = reader.ReadInt32(),
							Offset = reader.ReadInt64()
						});
					}
				}
			}
		}

		public async Task Load()
		{
			foreach (var id in _resources)
			{
				await _resourceManager.Load(this, id);
			}
		}

		public void Unload()
		{
			foreach (var id in _resources)
			{
				_resourceManager.Unload(id);
			}
		}

		public Stream Open(ResourceId resourceId)
		{
			if (_isBundle)
			{
				// Open sub resource in package bundle
				var resource = _resourceOffset[resourceId];

				var stream = File.OpenRead(_packagePath);
				return new StreamWrapper(stream, resource.Length, resource.Offset);
			}
			else
			{
				// Open actual file
				var resourcePath = Path.Combine(_path, resourceId.ToString());
				return File.OpenRead(resourcePath);
			}
		}

		class ResourceInfo
		{
			public int Length;
			public long Offset;
		}

		class StreamWrapper : Stream
		{
			private readonly Stream _baseStream;
			private readonly int _length;
			private readonly long _offset;

			public StreamWrapper(Stream baseStream, int length, long offset)
			{
				_baseStream = baseStream;

				_length = length;
				_offset = offset;

				_baseStream.Position = _offset;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				_baseStream.Dispose();
			}

			public override bool CanRead { get { return true; } }
			public override bool CanSeek { get { return true; } }
			public override bool CanWrite { get { return false; } }

			public override void Flush()
			{
				throw new NotImplementedException();
			}

			public override long Length { get { return _length; } }
			public override long Position
			{
				get { return _baseStream.Position - _offset; }
				set { _baseStream.Position = _offset + value; }
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (Position + count > _length)
					count = (int)Math.Max(_length - Position, 0);

				return _baseStream.Read(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				var newPosition = Position;
				switch (origin)
				{
					case SeekOrigin.Begin:
						newPosition = offset;
						break;
					case SeekOrigin.End:
						newPosition = _length - offset;
						break;
					case SeekOrigin.Current:
						newPosition = Position + Length;
						break;
				}

				if (newPosition < 0)
					newPosition = 0;
				else if (newPosition > _length)
					newPosition = _length;

				Position = newPosition;
				return Position;
			}

			public override void SetLength(long value)
			{
				throw new NotImplementedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}
		}
	}
}
