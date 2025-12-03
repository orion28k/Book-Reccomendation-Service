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

        await _userRepository.AddAsync(user);
        return id;
    }

    public async Task<Guid> UpdateUser(UpdateUserDto updateUserDto, Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            throw new Exception("User Not Found");
        }
        if (updateUserDto.Username is not null) user.Username = updateUserDto.Username;
        if (updateUserDto.FirstName is not null) user.FirstName = updateUserDto.FirstName;
        if (updateUserDto.LastName is not null)user.LastName = updateUserDto.LastName;
        if (updateUserDto.Email is not null) user.Email = updateUserDto.Email;
        if (updateUserDto.PreferredGenres is not null) user.PreferredGenres = updateUserDto.PreferredGenres.ToList();
        user.UpdatedAt = updateUserDto.UpdatedAt;

        await _userRepository.UpdateAsync(user);
        return user.Id;
    }

    public async Task UpdatePreferredGenresAsync(Guid id, IEnumerable<string> preferredGenres)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return;
        }
        user.updatePreferredGenres(preferredGenres);
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return;
        }
        await _userRepository.DeleteAsync(id);
    }

    private static UserDto MapToDto(User user) => new UserDto(
        user.Id,
        user.Username,
        user.FirstName,
        user.LastName,
        user.Email,
        user.PreferredGenres,
        user.CreatedAt
    );
}