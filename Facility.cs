﻿using System;
using System.Collections.Generic;

namespace mongodb
{
	public class Facility
	{
		public Guid Id { get; set; }
		public List<AttributeValue> AttributesValues { get; set; }
		public List<ParameterValue> ParametersValues { get; set; }
	}
}