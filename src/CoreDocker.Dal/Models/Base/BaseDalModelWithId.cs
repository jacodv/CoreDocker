﻿using System;
using MainSolutionTemplate.Dal.Models.Interfaces;

namespace MainSolutionTemplate.Dal.Models
{
	public abstract class BaseDalModelWithId : BaseDalModel, IBaseDalModelWithId
	{
		public BaseDalModelWithId()
		{
		}

		public string Id { get; set; }
	}
}