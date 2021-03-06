﻿using System;

namespace CoreDocker.Dal.Models.Base
{
	public abstract class BaseDalModel : IBaseDalModel
	{
		public BaseDalModel()
		{
			CreateDate = DateTime.Now;
			UpdateDate = DateTime.Now;
		}

		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
	}
}