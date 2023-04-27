namespace RPM_Project_Backend.Tests;

public static partial class TestValues
{
    public static readonly IReadOnlyList<Role> Roles = new List<Role>()
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
    
    public static readonly IReadOnlyList<User> Users = new List<User>()
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

public static partial class TestValues
{
    public static IReadOnlyList<Genre> Genres = new List<Genre>()
    {
        new ()
        {
            Id = 1, Name = "root category"
        },
        new ()
        {
            Id = 2, Name = "simple category"
        },
        new ()
        {
            Id = 3, Name = "category"
        },
    };
    
    public static IReadOnlyList<Product> Products = new List<Product>()
    {
        new ()
        {
            Id = 1,
            Cost = 1000,
            Discount = 0,
            Name = "First product",
            Rating = 5,
            SellerId = 3,
            Genres = Genres,
            Seller = Users.First(u => u.Id == 3)
        },
        new ()
        {
            Id = 2,
            Cost = 2000,
            Discount = 20,
            Name = "Second product",
            Rating = 3.2,
            SellerId = 3,
            Genres = Genres,
            Seller = Users.First(u => u.Id == 3)
        },
        new ()
        {
            Id = 3,
            Cost = 3000,
            Discount = 10,
            Name = "Third product",
            Rating = 4.6,
            SellerId = 3,
            Genres = Genres,
            Seller = Users.First(u => u.Id == 3)
        },
    };

    public static Product SingleProduct = new()
    {
        Id = 4,
        Cost = 4000,
        Discount = 20,
        Name = "Single product",
        Rating = 4.2,
        SellerId = 2,
        Genres = Genres,
        Seller = Users.First(u => u.Id == 3)
    };
}

public static partial class TestValues
{
    public static IReadOnlyList<Image> Images = new List<Image>()
    {
        new ()
        {
            Id = 1, 
        },
        new ()
        {
            Id = 2, 
        },
        new ()
        {
            Id = 3, 
        },
    };

    public static Image SingleImage = new()
    {
        Id = 4,
    };
}