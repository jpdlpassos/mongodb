﻿using System;

namespace mongodb
{
	public abstract class AttributeValue<T> : AttributeValue
	{
		public T Value { get; set; }
	}

	public abstract class AttributeValue
	{
		public Guid Id { get; set; }
		public Guid AttributeId { get; set; }
	}
}