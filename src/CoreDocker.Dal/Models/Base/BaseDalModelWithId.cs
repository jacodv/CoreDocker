﻿using System;
using CoreDocker.Dal.Models.Interfaces;

namespace CoreDocker.Dal.Models
{
	public abstract class BaseDalModelWithId : BaseDalModel, IBaseDalModelWithId
	{
		public BaseDalModelWithId()
		{
		}

		public string Id { get; set; }
	}
}