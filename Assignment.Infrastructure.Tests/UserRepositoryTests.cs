namespace Assignment.Infrastructure.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment.Core;
using Microsoft.EntityFrameworkCore.Sqlite;


public class UserRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly UserRepository _repository;
    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        var userForWI = new User("BobMichael", "bobMichael@gmail.com" ) {Id = 3};
        context.Users.AddRange(new User("Bob", "bob@gmail.com") { Id = 1 }, new User("Michael", "Michael@gmail.com") { Id = 2 }, userForWI);
        context.Items.Add(new WorkItem("Bob Project") { Id = 1, Description = "bla bla bla", AssignedTo = userForWI});
        context.SaveChanges();

        _context = context;
        _repository = new UserRepository(_context);
    }

    [Fact]
    public void createUserreturnsCreatedWithUser() {
    //Arrange
    var user = new UserDTO(4, "Clara", "Clara@gmail.com");
    //Act
    var (response, userId) = _repository.Create(new UserCreateDTO("Clara", "Clara@gmail.com"));
    //Assert
    response.Should().Be(Response.Created);

    userId.Should().Be(user.Id);
    }

    [Fact]
    public void createUserreturnsConflictIfUserExists() {
    //Arrange
    //Act
    var (response, userId) = _repository.Create(new UserCreateDTO("Bob", "bob@gmail.com"));
    //Assert
    response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void readGivenIdReturnsUser() {
    //Arrange
    var expected = new UserDTO(1, "Bob", "bob@gmail.com");
    //Act
    var actual = _repository.Find(1);
    //Assert
    actual.Should().Be(expected);
    }

    [Fact]
    public void readGivenNonExistingIdReturnsNull() {
    //Arrange
    //Act
    var actual = _repository.Find(10);
    //Assert
    actual.Should().Be(null);
    }

    [Fact]
    public void readAllReturnsAllUsers() {
    //Arrange
    //Act
    var actual = _repository.Read().Count();
    //Assert
    actual.Should().Be(3);
    }

    [Fact]
    public void updateReturnsUpdatedWhenGivenExistingUser() {
    //Arrange
    var user = new UserUpdateDTO(1, "Bob,", "BobsnyeMail@gmail.com");
    //Act
    var Response = _repository.Update(user);
    var foundUser = _repository.Find(1);
    var updatedEmail = foundUser.Email;
    //Assert
    Response.Should().Be(Response.Updated);
    updatedEmail.Should().Be("BobsnyeMail@gmail.com");
    }

    [Fact]
    public void updateReturnsNotFoundWhenGivenNonExistingUser() {
    //Arrange
    var user = new UserUpdateDTO(7, "Forkert Bruger,", "Forkertbruger@gmail.com");
    //Act
    var Response = _repository.Update(user);
    //Assert
    Response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void DeleteReturnsDeletedWhenExistingUserIdGiven() {
    //Arrange
    //Act
    var Response = _repository.Delete(1);
    var ReadResponse = _repository.Find(1);
    //Assert
    Response.Should().Be(Response.Deleted);
    ReadResponse.Should().Be(null);
    }

    [Fact]
    public void DeleteReturnsNotFoundWhenNonExistingUserIdGiven() {
    //Arrange
    //Act
    var Response = _repository.Delete(11);
    //Assert
    Response.Should().Be(Response.NotFound);
    }

    public void Dispose() {
        _context.Dispose();
    }
}

