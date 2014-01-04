
using System.Collections.Generic;
using System.Text;
using Com.Morln.Game;
using ProtoBuffer;
namespace Com.Morln.Packet
{
	/// <summary>
	/// </summary>
	public class Packet : ISendable, IReceiveable
	{
		private bool HasAFooEnum{get;set;}
		private FooEnum aFooEnum;
		/// <summary>
		/// </summary>
		public FooEnum AFooEnum
		{
			get
			{
				return aFooEnum;
			}
			set
			{
				HasAFooEnum = true;
				aFooEnum = value;
			}
		}

		private bool HasFoo{get;set;}
		private Foo foo;
		/// <summary>
		/// </summary>
		public Foo Foo
		{
			get
			{
				return foo;
			}
			set
			{
				if(value != null)
				{
					HasFoo = true;
					foo = value;
				}
			}
		}

		private List<int> valueList;
		/// <summary>
		/// </summary>
		public List<int> ValueList
		{
			get
			{
				return valueList;
			}
			set
			{
				if(value != null)
				{
					valueList = value;
				}
			}
		}

		/// <summary>
		/// </summary>
		public Packet()
		{
			ValueList = new List<int>();
		}

		/// <summary>
		/// </summary>
		public Packet
		(
			FooEnum aFooEnum,
			Foo foo
		):this()
		{
			AFooEnum = aFooEnum;
			Foo = foo;
		}
		private void CheckRequiredFields()
		{
			if( !HasAFooEnum)
			{
				throw new ProtoBufferException("missing required field,name:" + "a_foo_enum" +",fieldNumber:" +1);
			}
			if( !HasFoo)
			{
				throw new ProtoBufferException("missing required field,name:" + "foo" +",fieldNumber:" +2);
			}
		}
		public byte[] GetProtoBufferBytes()
		{
			CheckRequiredFields();
			ProtoBufferWriter writer = new ProtoBufferWriter();
			writer.Write(1,(int)AFooEnum);
			writer.Write(2,Foo);
			writer.Write(3,ValueList);
			return writer.GetProtoBufferBytes();
		}
		public void ParseFrom(byte[] buffer)
		{
			 ParseFrom(buffer, 0, buffer.Length);
		}
		public void ParseFrom(byte[] buffer, int offset, int size)
		{
			if (buffer == null) return;
			ProtoBufferReader reader = new ProtoBufferReader(buffer,offset,size);
			foreach (ProtoBufferObject obj in reader.ProtoBufferObjs)
			{
				switch (obj.FieldNumber)
				{
					case 1:
						AFooEnum = (FooEnum)((int)obj.Value);
						break;
					case 2:
						Foo = new Foo();
						Foo.ParseFrom(obj.Value);
						break;
					case 3:
						ValueList.Add(obj.Value);
						break;
					default:
						break;
				}
			}
			CheckRequiredFields();
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("AFooEnum ; " + AFooEnum + ",");
			sb.Append("Foo ; " + Foo + ",");
			sb.Append("ValueList : [");
			for(int i = 0; i < ValueList.Count;i ++)
				if(i == ValueList.Count -1)
				{
					sb.Append(ValueList[i]);
				}else
					sb.Append(ValueList[i] + ",");
				{
				}
			sb.Append("]");
			sb.Append("}");
			return sb.ToString();
		}
	}
}


