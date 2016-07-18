using System;
using System.IO;
using NUnit.Framework;

namespace Velox.Core.Test
{
	[SetUpFixture]
	public class MySetUpClass
	{
		public class IOHandler : FileIOHandler
		{
			public override void AppendAllText(string path, string s)
			{
				File.AppendAllText(path, s);
			}

			public override void Delete(string path)
			{
				File.Delete(path);
			}

			public override bool FileExists(string path)
			{
				return File.Exists(path);
			}

			public override Stream OpenReadStream(string path, bool exclusive)
			{
				return File.OpenRead(path);
			}

			public override Stream OpenWriteStream(string path, bool exclusive, bool create)
			{
				return File.OpenWrite(path);
			}

			public override string[] ReadAllLines(string path)
			{
				return File.ReadAllLines(path);
			}

			public override string ReadAllText(string path)
			{
				return File.ReadAllText(path);
			}

			public override void WriteAllText(string path, string s)
			{
				File.WriteAllText(path, s);
			}
		}

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			FileIO.SetIOHandler(new IOHandler());
		}
	}
}

