
﻿using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuffer;
namespace Com.Morln.Game
{
	/// <summary>
	/// </summary>
	public class Foo : ISendable, IReceiveable
	{
		private bool HasRequiredEnum{get;set;}
		private FooEnum requiredEnum;
		/// <summary>
		/// </summary>
		public FooEnum RequiredEnum
		{
			get
			{
				return requiredEnum;
			}
			set
			{
				HasRequiredEnum = true;
				requiredEnum = value;
			}
		}

		public bool HasOptionalEnum{get;private set;}
		private FooEnum optionalEnum;
		/// <summary>
		/// </summary>
		public FooEnum OptionalEnum
		{
			get
			{
				return optionalEnum;
			}
			set
			{
				HasOptionalEnum = true;
				optionalEnum = value;
			}
		}

		private List<FooEnum> repeatedEnumList;
		/// <summary>
		/// </summary>
		public List<FooEnum> RepeatedEnumList
		{
			get
			{
				return repeatedEnumList;
			}
			set
			{
				if(value != null)
				{
					repeatedEnumList = value;
				}
			}
		}

		/// <summary>
		/// </summary>
		public Foo()
		{
			RepeatedEnumList = new List<FooEnum>();
		}

		/// <summary>
		/// </summary>
		public Foo
		(
			FooEnum requiredEnum
		):this()
		{
			RequiredEnum = requiredEnum;
		}
		private void CheckRequiredFields()
		{
			if( !HasRequiredEnum)
			{
				throw new ProtoBufferException("missing required field,name:" + "required_enum" +",fieldNumber:" +1);
			}
		}
		public byte[] GetProtoBufferBytes()
		{
			CheckRequiredFields();
			ProtoBufferWriter writer = new ProtoBufferWriter();
			writer.Write(1,(int)RequiredEnum);
			writer.Write(2,(int)OptionalEnum);
			foreach(FooEnum v in RepeatedEnumList)
			{
				writer.Write(3,(int)v);
			}
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
						RequiredEnum = (FooEnum)((int)obj.Value);
						break;
					case 2:
						OptionalEnum = (FooEnum)((int)obj.Value);
						break;
					case 3:
						RepeatedEnumList.Add((FooEnum)((int)obj.Value));
						break;
					default:
						break;
				}
			}
			CheckRequiredFields();
		}
	}
}


