using System;
using System.IO;
using NUnit.Framework;

namespace Iridium.Core.Test
{
	[SetUpFixture]
	public class MySetUpClass
	{
		public class IOHandler : IFileIOHandler
		{
			public virtual void AppendAllText(string path, string s)
			{
				File.AppendAllText(path, s);
			}

			public virtual void Delete(string path)
			{
			    try
			    {
			        File.Delete(path);
			    }
                catch (Exception ex)
                {
                    throw new FileIOException(ex.Message, ex);
                }
            }

            public void CreateFolder(string path, bool deep = false)
		    {
		        try
		        {
		            Directory.CreateDirectory(path);
		        }
		        catch (Exception ex)
		        {
		            throw new FileIOException(ex.Message, ex);
		        }
		    }

		    public void DeleteFolder(string path)
		    {
		        try
		        {
		            Directory.Delete(path);
		        }
                catch (Exception ex)
                {
                    throw new FileIOException(ex.Message, ex);
                }
            }

            public bool FolderExists(string path)
            {
                return Directory.Exists(path);
            }

		    public virtual bool FileExists(string path)
			{
				return File.Exists(path);
			}

		    public virtual Stream OpenReadStream(string path, bool exclusive)
			{
				return File.OpenRead(path);
			}

			public virtual Stream OpenWriteStream(string path, bool exclusive, bool create)
			{
				return File.OpenWrite(path);
			}

			public virtual string[] ReadAllLines(string path)
			{
				return File.ReadAllLines(path);
			}

		    public byte[] ReadAllBytes(string path)
		    {
		        return File.ReadAllBytes(path);
		    }

		    public virtual string ReadAllText(string path)
			{
				return File.ReadAllText(path);
			}

			public virtual void WriteAllText(string path, string s)
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

