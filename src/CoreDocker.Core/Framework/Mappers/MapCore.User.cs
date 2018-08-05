using AutoMapper;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
	{
        public static void CreateUserMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserReference>();
            cfg.CreateMap<User, Registered>();
        }

        public static UserReference ToReference(this User user, UserReference userReference = null)
        {
            return Mapper.Map(user, userReference);
        }

	    public static Registered ToRegistered(this User user, Registered registered = null)
        {
            return Mapper.Map(user, registered);
        }
	}
}