using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UnitTests.Planning
{
    public class ObjectiveHandlersTests
    {
        [Fact]
        public async Task CreateObjectiveHandler_SetsLevel_OnCreate()
        {
            // Arrange
            var repoMock = new Mock<IObjectiveRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Objective>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var handler = new CreateObjectiveCommandHandler(repoMock.Object);

            var cmd = new CreateObjectiveCommand(
                sport: Sport.Basketball,
                name: "Test Obj",
                description: "desc",
                objectiveCategoryId: Guid.NewGuid(),
                objectiveSubcategoryId: null,
                level: 4
            );

            // Act
            var id = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.Is<Objective>(o => o.Level == 4 && o.Name == "Test Obj"), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task UpdateObjectiveHandler_UpdatesLevel()
        {
            // Arrange
            var existing = new Objective(Guid.NewGuid(), ContentOwnership.User, Sport.Basketball, "Existing", "", Guid.NewGuid(), null) { Level = 2 };
            var repoMock = new Mock<IObjectiveRepository>();
            repoMock.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Objective>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

            var handler = new UpdateObjectiveCommandHandler(repoMock.Object);

            var cmd = new UpdateObjectiveCommand(existing.Id, "Existing updated", "desc", 5);

            // Act
            await handler.Handle(cmd, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.UpdateAsync(It.Is<Objective>(o => o.Level == 5 && o.Name == "Existing updated"), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(5, existing.Level);
        }
    }
}
