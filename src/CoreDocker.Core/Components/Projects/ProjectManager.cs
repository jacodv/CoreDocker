using System.Collections.Generic;
using MainSolutionTemplate.Core.BusinessLogic.Components.Interfaces;
using MainSolutionTemplate.Dal.Models;
using MainSolutionTemplate.Dal.Persistance;
using Microsoft.Extensions.Logging;


namespace MainSolutionTemplate.Core.BusinessLogic.Components
{
    public class ProjectManager : BaseManager<Project>, IProjectManager
    {
        public ProjectManager(BaseManagerArguments baseManagerArguments , ILogger<ProjectManager> logger ) : base(baseManagerArguments, logger)
        {
          
        }

        #region Overrides of BaseManager<Project>

        protected override IRepository<Project> Repository
        {
            get { return _generalUnitOfWork.Projects; }
        }

        #endregion
    }
}

/* scaffolding [
    {
      "FileName": "IocCoreBase.cs",
      "Indexline": "As<IUserManager>",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "builder.RegisterType<ProjectManager>().As<IProjectManager>();"
      ]
    },
    {
      "FileName": "MongoGeneralUnitOfWork.cs",
      "Indexline": "Projects = ",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Projects = new MongoRepository<Project>(database);"
      ]
    },
    {
      "FileName": "IGeneralUnitOfWork.cs",
      "Indexline": "Projects",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "IRepository<Project> Projects { get; }"
      ]
    },
    {
      "FileName": "MongoGeneralUnitOfWork.cs",
      "Indexline": "Projects { get; private set; }",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "public IRepository<Project> Projects { get; private set; }"
      ]
    },
  {
      "FileName": "InMemoryGeneralUnitOfWork.cs",
      "Indexline": "Projects =",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Projects = new FakeRepository<Project>();"
      ]
    },
    {
      "FileName": "InMemoryGeneralUnitOfWork.cs",
      "Indexline": "Projects { get",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "public IRepository<Project> Projects { get; private set; }"
      ]
    }
] scaffolding */