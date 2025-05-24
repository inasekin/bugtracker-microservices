using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using Xunit;
using FluentAssertions;
using AutoFixture.Xunit2;
using System.Threading.Tasks;
using BugTracker.DataAccess;
using BugTracker.Domain;
using Bugtracker.WebHost.Controllers;
using Bugtracker.WebHost.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Bugtracker.Tests.Projects.Controllers
{
    public class CreateProjectAsyncTests
    {
        private readonly Mock<IRepository<Project>> _projectsRepositoryMock;
        private readonly ProjectsController _projectsController;
        private readonly IFixture _fixture;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public CreateProjectAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));
            _unitOfWork = _fixture.Freeze<Mock<IUnitOfWork>>();
            _projectsRepositoryMock = _fixture.Freeze<Mock<IRepository<Project>>>();
            _projectsController = _fixture.Build<ProjectsController>().OmitAutoProperties().Create();
        }

        [Theory, AutoData]
        public async Task CreateProjectAsync_ProjectRandom_ReturnsProject(ProjectRequest request)
        {
            // Arrange

            // Act
            var result = await _projectsController.CreateProjectAsync(request);

            // Assert
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            //result.Value.Name.Should().Be(request.Name);

            _unitOfWork.Verify(i => i.SaveChangesAsync(), Times.Once());
            _projectsRepositoryMock.Verify(i => i.Add(It.IsAny<Project>()), Times.Once());
        }
    }
}
