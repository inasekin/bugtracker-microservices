using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using BugTracker.DataAccess;
using BugTracker.Domain;
using Bugtracker.WebHost.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bugtracker.Tests.Projects.Controllers
{
    public class DeleteProjectAsyncTests
    {
        private readonly Mock<IRepository<Project>> _projectsRepositoryMock;
        private readonly ProjectsController _projectsController;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public DeleteProjectAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _unitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
            _projectsRepositoryMock = fixture.Freeze<Mock<IRepository<Project>>>();
            _projectsController = fixture.Build<ProjectsController>().OmitAutoProperties().Create();
        }
        
        [Fact]
        public async Task DeleteProjectAsync_ProjectIsNotFound_ReturnsNotFound()
        {
            //// Arrange
            var projectId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");

            _projectsRepositoryMock.Setup(repo => repo.GetAsync(projectId))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _projectsController.DeleteProjectAsync(projectId);

            //// Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteProjectAsync_ProjectGood_ReturnsNoContent()
        {
            // Arrange
            var projectId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");

            // Act
            var result = await _projectsController.DeleteProjectAsync(projectId);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();

            _unitOfWork.Verify(i => i.SaveChangesAsync(), Times.Once());
            _projectsRepositoryMock.Verify(i => i.Remove(It.IsAny<Project>()), Times.Once());
        }
    }
}
