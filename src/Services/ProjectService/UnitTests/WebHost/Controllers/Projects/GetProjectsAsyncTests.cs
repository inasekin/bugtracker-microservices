using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bugtracker.WebHost.Contracts;
using Bugtracker.WebHost.Controllers;
using Bugtracker.WebHost.Mapping;
using BugTracker.DataAccess;
using BugTracker.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bugtracker.Tests.Projects.Controllers
{
    public class GetProjectsAsyncTests
    {
        private readonly Mock<IRepository<Project>> _projectsRepositoryMock;
        private readonly ProjectsController _projectsController;

        public GetProjectsAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProjectMappingProfile>();
            });
            mapperConfiguration.AssertConfigurationIsValid();
            fixture.Register<IMapper>(() => new Mapper(mapperConfiguration));

            _projectsRepositoryMock = fixture.Freeze<Mock<IRepository<Project>>>();
            _projectsController = fixture.Build<ProjectsController>().OmitAutoProperties().Create();
        }

        public Project CreateBaseProject()
        {
            var project = new Project()
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "TestProject",
                Description = "TestDescription",
                UserRoles = new List<ProjectUserRoles>(),
                IssueCategories = new List<ProjectIssueCategory>(),
                Versions = new List<ProjectVersion>(),
                IssueTypes = new List<ProjectIssueType>(),
                ParentProjectId = null
            };
            return project;
        }

        [Fact]
        public async Task GetProjectsAsync_ProjectsExists_ReturnsProjects()
        {
            // Arrange
            Project project1 = CreateBaseProject();
            Project project2 = CreateBaseProject();
            project2.Name = "TestProject2";
            project2.Description = "TestDescription2";

            _projectsRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Project>() { project2, project1 }); // Back order collection

            // Act
            var actionResult = await _projectsController.GetProjectsAsync();

            // Assert
            actionResult.Should().NotBeNull();
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            IEnumerable<ProjectResponse> r = ((OkObjectResult)actionResult.Result).Value as IEnumerable<ProjectResponse>;
            r.Count().Should().Be(2);
            r.First().Name.Should().Be("TestProject"); // Check ordered
        }

        [Fact]
        public async Task GetProjectByIdAsync_ProjectExist_ReturnsProject()
        {
            // Arrange
            var projectId = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8");
            Project project = CreateBaseProject();

            _projectsRepositoryMock.Setup(repo => repo.GetAsync(projectId))
                .ReturnsAsync(project);

            // Act
            ActionResult<ProjectResponse> actionResult = await _projectsController.GetProjectAsync(projectId);

            // Assert
            actionResult.Should().NotBeNull();
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            ProjectResponse r = ((OkObjectResult)actionResult.Result).Value as ProjectResponse;

            r.Name.Should().Be("TestProject");
            r.Description.Should().Be("TestDescription");
        }

        [Fact]
        public async Task GetProjectByIdAsync_ProjectNotExist_ReturnsNotFound()
        {
            // Arrange
            var projectId = Guid.Parse("00004823-8226-4273-b063-1a95f3cc1df8");
            _projectsRepositoryMock.Setup(repo => repo.GetAsync(projectId))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _projectsController.GetProjectAsync(projectId);

            // Assert
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
