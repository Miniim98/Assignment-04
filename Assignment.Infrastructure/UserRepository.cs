namespace Assignment.Infrastructure;

using Assignment.Core;
using Microsoft.EntityFrameworkCore;
public class UserRepository : IUserRepository
{
    
    private readonly KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int UserId) Create(UserCreateDTO user) {
        var entity = _context.Users.FirstOrDefault(u => u.Name == user.Name);
        
        if (entity is null) {
            entity = new User(user.Name, user.Email);
            _context.Users.Add(entity);
            _context.SaveChanges();
        } else {
            return (Response.Conflict, 0);
        }
        
        return (Response.Created, entity.Id);
    }

    public IReadOnlyCollection<UserDTO> Read() {
        var users = from u in _context.Users
                                orderby u.Name
                                select new UserDTO(u.Id, u.Name, u.Email);

        return users.ToArray();
    }

    public UserDTO Find(int userId) {
        var users = from u in _context.Users
                     where u.Id == userId
                     select new UserDTO(u.Id, u.Name, u.Email);

        return users.FirstOrDefault();
    }

    public Response Update(UserUpdateDTO user) {
    var entity = _context.Users.Find(user.Id);
     
             if (entity is null)
        {
            return Response.NotFound;
        }
        else if (_context.Users.FirstOrDefault(u => u.Id != user.Id && u.Name == user.Name) != null)
        {
            return Response.Conflict;
        }
        else
        {
            entity.Name = user.Name;
            entity.Email = user.Email;
            _context.SaveChanges();
            return Response.Updated;
        }  
    }

    public Response Delete(int userId, bool force = false) {
        var user = _context.Users.Include(u => u.Items).FirstOrDefault(u => u.Id == userId);

        if (user is null) {
            return Response.NotFound;
        } else if (user.Items.Any()) {
            return Response.Conflict;
        } else {
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Response.Deleted;
        }
    } 
}
