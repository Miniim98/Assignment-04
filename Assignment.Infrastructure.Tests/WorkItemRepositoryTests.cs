namespace Assignment.Infrastructure.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment.Core;

public sealed class WorkItemRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly WorkItemRepository _repository;
    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        var User1 = new User ("Superman", "superman123@gmail.com" );
        var User2 = new User ("Batman", "Batman123@gmail.com" );
        var tag1 = new Tag("tag number 1");
        var tag2 = new Tag("tag number 2");
        var WorkItem1 = new WorkItem("Title work Item 1"){AssignedTo = User1, Description = "Detailed description!", Tags = new HashSet<Tag>{tag1}};
        var WorkItem2 = new WorkItem("Title work Item 2"){AssignedTo = User1, Description = "Detailed description!", Tags = new HashSet<Tag>{tag2}};
        var WorkItem3 = new WorkItem("Title work Item 3"){AssignedTo = User2, Description = "Detailed description!", Tags = new HashSet<Tag>{tag1}};
        context.Items.AddRange(WorkItem1, WorkItem2, WorkItem3);
        context.Users.Add(User1);
        context.Tags.Add(tag1);
        context.SaveChanges();

        _context = context;
        _repository = new WorkItemRepository(_context);
    }

    [Fact]
    public void Test_create_workitem_returns_created()
    {
        //Arrange
        
        //Act
        var (response, id) = _repository.Create(new WorkItemCreateDTO("Work Item 4", 1, "Very detailed description", new string[]{"tag number 1"}));

        //Assert
        response.Should().Be(Response.Created);
        id.Should().Be(4);
        
    }

    [Fact]
    public void Test_create_workitem_returns_NotFound()
    {
        //Arrange
        
        //Act
        var (response, id) = _repository.Create(new WorkItemCreateDTO("Work Item 4", 4, "Very detailed description", new string[]{"tag number 1"}));

        //Assert
        response.Should().Be(Response.NotFound);
        id.Should().Be(0);
        
    }

    [Fact]
    public void Test_read_all_workitems()
    {
        //Arrange
        _repository.Create(new WorkItemCreateDTO("Work Item 4", 1, "Very detailed description", new string[]{"tag number 1"}));

        //Act
        var res = _repository.Read();

        //Assert
        res.Should().BeEquivalentTo(new [] {new WorkItemDTO(1, "Title work Item 1", "Superman", new string[] {"tag number 1"}, State.New ),
        new WorkItemDTO(2, "Title work Item 2", "Superman", new string[] {"tag number 2"}, State.New ), 
        new WorkItemDTO(3, "Title work Item 3", "Batman", new string[] {"tag number 1"}, State.New ), 
        new WorkItemDTO(4, "Work Item 4", "Superman", new string[] {"tag number 1"}, State.New )});
        
    }

    [Fact]
    public void Test_read_all_removed()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new WorkItemCreateDTO("This is a very awesome work item", 2, "Very awesome", new []{"tag number 1"}));
        var (response2, id2) = _repository.Create(new WorkItemCreateDTO("This is an awesome work item", 1, "Very awesome", new []{"tag number 1"}));
        //Act
        _repository.Update(new WorkItemUpdateDTO(id1, "This is a very awesome work item", 2,"Very awesome", new []{"tag number 1"}, State.Active));
        _repository.Delete(id1);
        _repository.Update(new WorkItemUpdateDTO(id2, "This is an awesome work item", 1,"Very awesome", new []{"tag number 1", "tag number 2"}, State.Active));
        _repository.Delete(id2);
        var res = _repository.ReadRemoved();

        //Assert
        res.Should().BeEquivalentTo(new [] {new WorkItemDTO(id1, "This is a very awesome work item", "Batman", new []{"tag number 1"}, State.Removed), 
        new WorkItemDTO(id2, "This is an awesome work item", "Superman", new []{"tag number 1", "tag number 2"}, State.Removed)});
     
        
    }

    [Fact]
    public void Test_read_all_by_tag()
    {
        //Arrange
        
        //Act
        var res = _repository.ReadByTag("tag number 1");

        //Assert
        res.Should().BeEquivalentTo(new [] {new WorkItemDTO(1, "Title work Item 1", "Superman", new string[] {"tag number 1"}, State.New ), 
        new WorkItemDTO(3, "Title work Item 3", "Batman", new string[] {"tag number 1"}, State.New )});
        
    }

    [Fact]
    public void Test_read_all_by_user()
    {
        //Arrange
        var (response, id) = _repository.Create(new WorkItemCreateDTO("This is an awesome work item", 2, "Very awesome", new []{"tag number 1"}));
        //Act
        var res = _repository.ReadByUser(2);

        //Assert
        res.Should().BeEquivalentTo(new [] {new WorkItemDTO(3, "Title work Item 3", "Batman", new string[] {"tag number 1"}, State.New ),
        new WorkItemDTO(id, "This is an awesome work item", "Batman", new string[] {"tag number 1"}, State.New ) 
        });
        
    }

    [Fact]
    public void Test_read_all_by_state()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new WorkItemCreateDTO("This is a very awesome work item", 2, "Very awesome", new []{"tag number 1"}));
        var (response2, id2) = _repository.Create(new WorkItemCreateDTO("This is an awesome work item", 2, "Very awesome", new []{"tag number 1"}));
        _repository.Update(new WorkItemUpdateDTO(id1, "This is a very awesome work item", 2, "Very awesome", new []{"tag number 1"}, State.Closed));
        _repository.Update(new WorkItemUpdateDTO(id2, "This is an awesome work item", 2, "Very awesome", new []{"tag number 1"}, State.Closed));
        //Act
        var res = _repository.ReadByState(State.Closed);

        //Assert
        res.Should().BeEquivalentTo(new []{new WorkItemDTO(id1, "This is a very awesome work item", "Batman", new []{"tag number 1"}, State.Closed), new WorkItemDTO(id2, "This is an awesome work item", "Batman", new []{"tag number 1"}, State.Closed)});
        
    }

    [Fact]
    public void Test_read_one_workitem()
    {
        //Arrange
         _repository.Create(new WorkItemCreateDTO("Work Item 4", 1, "Very detailed description", new string[]{"tag number 1"}));
        //Act
        var res = _repository.Find(4);

        //Assert
        res.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        res.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        res.Id.Should().Be(4);
        res.Title.Should().Be("Work Item 4");
        
    }

    [Fact]
    public void Test_update_workitem_updateState()
    {
        //Arrange
        var (response, id) = _repository.Create(new WorkItemCreateDTO("This is an awesome work item", 2, "Very awesome", new []{"tag number 1"}));

        //Act
        var res = _repository.Update(new WorkItemUpdateDTO(id, "This is an awesome work item", 2,"Very awesome", new []{"tag number 1", "tag number 2"}, State.Active));
        var resTags = _repository.ReadByTag("tag number 2");
        var resRead = _repository.Find(id);

        //Assert
        resRead.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        res.Should().Be(Response.Updated);
        resTags.Should().BeEquivalentTo(new [] {new WorkItemDTO(2, "Title work Item 2", "Superman", new string[] {"tag number 2"}, State.New ),
        new WorkItemDTO(id, "This is an awesome work item", "Batman", new string[]{"tag number 1", "tag number 2"}, State.Active)});
        
    }

    [Fact]
    public void Test_delete_workitem_returns_deleted()
    {
        //Arrange
        
        //Act
        var res = _repository.Delete(1);

        //Assert
        res.Should().Be(Response.Deleted);
        
    }

    [Fact]
    public void Test_delete_workitem_returns_updated_and_set_state_to_removed()
    {
        //Arrange
        var (response, id) = _repository.Create(new WorkItemCreateDTO("This is an awesome work item", 2, "Very awesome", new []{"tag number 1"}));
        //Act
        _repository.Update(new WorkItemUpdateDTO(id, "This is an awesome work item", 2,"Very awesome", new []{"tag number 1", "tag number 2"}, State.Active));
        var res = _repository.Delete(id);
        var readRes = _repository.Find(id);

        //Assert
        readRes.State.Should().Be(State.Removed);
        res.Should().Be(Response.Updated);

        
    }

    public void Dispose()
    {
        _context.Dispose();
    }

}

