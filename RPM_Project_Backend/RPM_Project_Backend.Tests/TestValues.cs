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
    public static IReadOnlyList<Product> Products = new List<Product>()
    {
        new ()
        {
            Id = 1,
            Cost = 1000,
            Discount = 0,
            Name = "First product",
            Description = "Some description",
            Rating = 5,
            SellerId = 3,
            Ts = DateTime.UtcNow,
            Seller = Users.First(u => u.Id == 3)
        },
        new ()
        {
            Id = 2,
            Cost = 2000,
            Discount = 20,
            Name = "Second product",
            Description = "Some description",
            Rating = 3.2,
            SellerId = 3,
            Ts = DateTime.UtcNow,
            Seller = Users.First(u => u.Id == 3)
        },
        new ()
        {
            Id = 3,
            Cost = 3000,
            Discount = 10,
            Name = "Third product",
            Description = "Some description",
            Rating = 4.6,
            SellerId = 3,
            Ts = DateTime.UtcNow,
            Seller = Users.First(u => u.Id == 3)
        },
    };

    public static Product SingleProduct = new()
    {
        Id = 4,
        Cost = 4000,
        Discount = 20,
        Name = "Single product",
        Description = "Some description",
        Rating = 4.2,
        SellerId = 2,
        Ts = DateTime.UtcNow,
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

public static partial class TestValues
{
    public static IReadOnlyList<Review> Reviews = new List<Review>()
    {
        new ()
        {
            Id = 1,
            Grade = 5.5,
            Text = "Some Review 1",
            ProductId = 1,
            Ts = DateTime.UtcNow,
            UserId = 1
        },
        new ()
        {
            Id = 2,
            Grade = 9.5,
            Text = "Some Review 2",
            ProductId = 2,
            Ts = DateTime.UtcNow,
            UserId = 1
        },
        new ()
        {
            Id = 3,
            Grade = 2.5,
            Text = "Some Review 3",
            ProductId = 3,
            Ts = DateTime.UtcNow,
            UserId = 1
        }
    };
    
    public static Review SingleReview = new ()
    {
        Id = 4,
        Grade = 10,
        Text = "Some Review 4",
        ProductId = 3,
        Ts = DateTime.UtcNow,
        UserId = 1
    };
}

public static partial class TestValues
{
    public static IReadOnlyList<Genre> Genres = new List<Genre>()
    {
        new ()
        {
            Id = 1,
            Name = "Some Genre Name 1"
        },
        new ()
        {
            Id = 2,
            Name = "Some Genre Name 2"
        },
        new ()
        {
            Id = 3,
            Name = "Some Genre Name 3"
        },
    };

    public static Genre SingleGenre = new()
    {
        Id = 4,
        Name = "Some Genre Name 4"
    };
}

public static partial class TestValues
{
    public static IReadOnlyList<Property> Properties = new List<Property>()
    {
        new ()
        {
            Id = 1,
            Name = "Some Property Name 1",
            Icon = "mdi-icon-1"
        },
        new ()
        {
            Id = 2,
            Name = "Some Property Name 2",
            Icon = "mdi-icon-2"
        },
        new ()
        {
            Id = 3,
            Name = "Some Property Name 3",
            Icon = "mdi-icon-3"
        }
    };

    public static Property SingleProperty = new()
    {
        Id = 4,
        Name = "Some Property Name 4",
        Icon = "mdi-icon-4"
    };
}

public static partial class TestValues
{
    public static IReadOnlyList<Payment> Payments = new List<Payment>()
    {
        new ()
        {
            Id = 1,
            UserId = 1,
            IsActive = true,
            Number = "1234567812345678",
            Cvc = "123",
            ExpirationDate = new DateTime(2024, 05, 1)
        },
        new ()
        {
            Id = 2,
            UserId = 2,
            IsActive = true,
            Number = "1234567812345678",
            Cvc = "456",
            ExpirationDate = new DateTime(2024, 06, 1)
        },
        new ()
        {
            Id = 3,
            UserId = 3,
            IsActive = true,
            Number = "1234567812345678",
            Cvc = "789",
            ExpirationDate = new DateTime(2024, 07, 1)
        }
    };

    public static Payment SinglePayment = new()
    {
        Id = 4,
        UserId = 1,
        IsActive = true,
        Number = "1234567812345678",
        Cvc = "023",
        ExpirationDate = new DateTime(2024, 04, 1)
    };
}