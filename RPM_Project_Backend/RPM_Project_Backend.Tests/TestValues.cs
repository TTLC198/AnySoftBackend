using Microsoft.AspNetCore.Identity;

namespace RPM_Project_Backend.Tests;

public static class TestValues
{
    public static IReadOnlyList<Role> Roles = new List<Role>()
    {
        new ()
        {
            Id = 1, Name = "admin"
        },
        new ()
        {
            Id = 2, Name = "user"
        },
        new ()
        {
            Id = 3, Name = "seller"
        },
    };
    
    public static IReadOnlyList<User> Users = new List<User>()
    {
        new ()
        {
            Id = 1,
            Email = "ex1@email.com",
            Login = "ex1",
            Password = "expass1",
            RoleId = 1,
            Role = Roles[0]
        },
        new ()
        {
            Id = 2,
            Email = "ex2@email.com",
            Login = "ex2",
            Password = "expass2",
            RoleId = 2,
            Role = Roles[1]
        },
        new ()
        {
            Id = 3,
            Email = "ex3@email.com",
            Login = "ex3",
            Password = "expass3",
            RoleId = 3,
            Role = Roles[2]
        },
    };

    public static User SingleUser = new()
    {
        Id = 4,
        Email = "user@email.com",
        Login = "user",
        Password = "userpass",
        RoleId = 2,
        Role = Roles[1]
    };
}