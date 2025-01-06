using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public class Packet : IDisposable
{
	public Packet()
	{
		this.buffer = new List<byte>();
		this.readPos = 0;
	}


	public Packet(int id)
	{
		this.buffer = new List<byte>();
		this.readPos = 0;
		this.Write(id);
	}


	public Packet(byte[] data, bool decompress = false)
	{
		this.buffer = new List<byte>();
		this.readPos = 0;
		this.SetBytes(data);
		if(decompress) Decompress();
	}


	public void Decompress()
	{
		MemoryStream ms = new MemoryStream();
		int msgLength = BitConverter.ToInt32(readableBuffer, 0);
		ms.Write(readableBuffer, 4, readableBuffer.Length - 4);

		byte[] dBuffer = new byte[msgLength];

		ms.Position = 0;
		GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
		zip.Read(dBuffer, 0, dBuffer.Length);

		this.buffer = new List<byte>();
		this.readPos = 0;
		this.SetBytes(dBuffer);
	}

	public void SetBytes(byte[] data)
	{
		this.Write(data);
		this.readableBuffer = this.buffer.ToArray();
	}


	public void WriteLength()
	{
		this.buffer.InsertRange(0, BitConverter.GetBytes(this.buffer.Count));
	}


	public void InsertInt(int value)
	{
		this.buffer.InsertRange(0, BitConverter.GetBytes(value));
	}


	public byte[] ToArray()
	{
		this.readableBuffer = this.buffer.ToArray();
		return this.readableBuffer;
	}
		
	public byte[] ToCompressedArray()
	{
		var b = buffer.ToArray();
		MemoryStream ms = new MemoryStream();
		GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
		zip.Write(b, 0, b.Length);
		zip.Close();
		ms.Position = 0;

		byte[] compressed = new byte[ms.Length];
		ms.Read(compressed, 0, compressed.Length);

		byte[] gzBuffer = new byte[compressed.Length + 4];
		Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
		Buffer.BlockCopy(BitConverter.GetBytes(b.Length), 0, gzBuffer, 0, 4);
		return gzBuffer;
	}


	public int Length()
	{
		return this.buffer.Count;
	}


	public int UnreadLength()
	{
		return this.Length() - this.readPos;
	}


	public void Reset(bool shouldReset = true)
	{
		if (shouldReset)
		{
			this.buffer.Clear();
			this.readableBuffer = null;
			this.readPos = 0;
		}
		else
		{
			this.readPos -= 4;
		}
	}


	public void Write(byte value)
	{
		this.buffer.Add(value);
	}


	public void Write(byte[] value)
	{
		this.buffer.AddRange(value);
	}


	public void Write(short value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(value));
	}


	public void Write(int value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(value));
	}


	public void Write(long value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(value));
	}


	public void Write(float value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(value));
	}


	public void Write(bool value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(value));
	}


	public void Write(string value)
	{

		var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
			
		this.Write(base64.Length);
		this.buffer.AddRange(Encoding.ASCII.GetBytes(base64));
	}


	public void Write(Vector3 value)
	{
		this.Write(value.x);
		this.Write(value.y);
		this.Write(value.z);
	}
		
	public void Write(Color value)
	{
		this.Write(value.r);
		this.Write(value.g);
		this.Write(value.b);
	}



	public void Write(Quaternion value)
	{
		this.Write(value.x);
		this.Write(value.y);
		this.Write(value.z);
		this.Write(value.w);
	}

	


	public byte ReadByte(bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			byte result = this.readableBuffer[this.readPos];
			if (moveReadPos)
			{
				this.readPos++;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'byte'!");
	}


	public byte[] ReadBytes(int length, bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			byte[] result = this.buffer.GetRange(this.readPos, length).ToArray();
			if (moveReadPos)
			{
				this.readPos += length;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'byte[]'!");
	}


	public short ReadShort(bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			short result = BitConverter.ToInt16(this.readableBuffer, this.readPos);
			if (moveReadPos)
			{
				this.readPos += 2;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'short'!");
	}


	public int ReadInt(bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			int result = BitConverter.ToInt32(this.readableBuffer, this.readPos);
			if (moveReadPos)
			{
				this.readPos += 4;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'int'!");
	}


	public long ReadLong(bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			long result = BitConverter.ToInt64(this.readableBuffer, this.readPos);
			if (moveReadPos)
			{
				this.readPos += 8;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'long'!");
	}


	public float ReadFloat(bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			float result = BitConverter.ToSingle(this.readableBuffer, this.readPos);
			if (moveReadPos)
			{
				this.readPos += 4;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'float'!");
	}


	public bool ReadBool(bool moveReadPos = true)
	{
		bool flag = this.buffer.Count > this.readPos;
		if (flag)
		{
			bool result = BitConverter.ToBoolean(this.readableBuffer, this.readPos);
			if (moveReadPos)
			{
				this.readPos++;
			}
			return result;
		}
		throw new Exception("Could not read value of type 'bool'!");
	}

	public string ReadString(bool moveReadPos = true)
	{
		string result;
		try
		{
			int num = this.ReadInt(true);
			string @string = Encoding.ASCII.GetString(this.readableBuffer, this.readPos, num);
			bool flag = moveReadPos && @string.Length > 0;
			if (flag)
			{
				this.readPos += num;
			}

			var r = Convert.FromBase64String(@string);
			result = Encoding.UTF8.GetString(r);
		}
		catch
		{
			throw new Exception("Could not read value of type 'string'!");
		}
		return result;
	}


	public Vector3 ReadVector3(bool moveReadPos = true)
	{
		return new Vector3(this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos));
	}
	public Vector3Int ReadVector3Int(bool moveReadPos = true)
	{
		return new Vector3Int(this.ReadInt(moveReadPos), this.ReadInt(moveReadPos), this.ReadInt(moveReadPos));
	}

	public Color ReadColor(bool moveReadPos = true)
	{
		return new Color(this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos));
	}


	public Quaternion ReadQuaternion(bool moveReadPos = true)
	{
		return new Quaternion(this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos), this.ReadFloat(moveReadPos));
	}

	protected virtual void Dispose(bool disposing)
	{
		bool flag = !this.disposed;
		if (flag)
		{
			if (disposing)
			{
				this.buffer = null;
				this.readableBuffer = null;
				this.readPos = 0;
			}
			this.disposed = true;
		}
	}
		
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	private List<byte> buffer;

	private byte[] readableBuffer;
		
	private int readPos;
		
	private bool disposed = false;
}
