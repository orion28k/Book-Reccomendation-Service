using BookRec.Application.Users.Dtos;
using BookRec.Application.Users.Interface;
using BookRec.Domain.UserModel;

namespace BookRec.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByUserAsync(string username)
    {
        var user = await _userRepository.GetByUserAsync(username);
        return user is null ? null : MapToDto(user);
    }

    public async Task<IReadOnlyList<string>> GetUserPreferredGenresAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Array.Empty<string>();
        }
        return user.PreferredGenres.ToList();
    }

    public async Task<Guid> AddUser(CreateUserDto createUserDto)
    {
        var id = Guid.NewGuid();

        var user = new User(
            id,
            createUserDto.Username,
            createUserDto.FirstName,
            createUserDto.LastName,
            createUserDto.Email,
            createUserDto.PreferredGenres,
            createUserDto.CreatedAt
        );

        await _userRepository.AddUser(user);
        return id;
    }

    public async Task UpdatePreferredGenresAsync(Guid id, IEnumerable<string> preferredGenres)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return;
        }
        user.updatePreferredGenres(preferredGenres);
        await _userRepository.UpdateUser(user);
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return;
        }
        await _userRepository.DeleteUser(user);
    }

    private static UserDto MapToDto(User user) => new UserDto(
        user.Id,
        user.Username,
        user.FirstName,
        user.LastName,
        user.Email,
        user.PreferredGenres,
        user.createdAt
    );
}