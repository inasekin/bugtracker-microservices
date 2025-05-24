using AutoFixture;
using AutoFixture.AutoMoq;
using BugTracker.DataAccess;
using BugTracker.Domain;
using Bugtracker.WebHost.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Threading.Tasks;
using System;
using AutoFixture.Xunit2;
using Bugtracker.WebHost.Contracts;
using FluentAssertions;

namespace Bugtracker.Tests.Projects.Controllers
{
    public class UpdateProjectAsyncTests
    {
        private readonly Mock<IRepository<Project>> _projectsRepositoryMock;
        private readonly ProjectsController _projectsController;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public UpdateProjectAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _unitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
            _projectsRepositoryMock = fixture.Freeze<Mock<IRepository<Project>>>();
            _projectsController = fixture.Build<ProjectsController>().OmitAutoProperties().Create();
        }

        [Theory, AutoData]
        public async Task UpdateProjectAsync_ObjectExist_ReturnsNoContent(ProjectRequest request)
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");

            // Act
            var result = await _projectsController.UpdateProjectAsync(partnerId, request);
 
            // Assert
            result.Should().BeAssignableTo<NoContentResult>();
        }
    }
}
