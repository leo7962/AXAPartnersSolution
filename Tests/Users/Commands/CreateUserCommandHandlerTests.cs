using Applications.Common.Interfaces;
using Applications.Users.Commands;
using Moq;
using Tests.Base;

namespace Tests.Users.Commands
{
    public class CreateUserCommandHandlerTests : TestBase
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new CreateUserCommandHandler(_mockContext.Object);
        }
    }
}
