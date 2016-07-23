using CoreDocker.Sdk;
using CoreDocker.Shared;
using MainSolutionTemplate.Sdk.RestApi.Base;
using MainSolutionTemplate.Shared.Interfaces.Shared;
using MainSolutionTemplate.Shared.Models;
using MainSolutionTemplate.Shared.Models.Reference;

namespace MainSolutionTemplate.Sdk.RestApi
{
    public class ProjectApiClient : BaseCrudApiClient<ProjectModel, ProjectCreateUpdateModel, ProjectReferenceModel>,
        IProjectControllerActions
    {
        public ProjectApiClient(CoreDockerClient dockerClient)
            : base(dockerClient, RouteHelper.ProjectController)
        {
        }

    }
}
