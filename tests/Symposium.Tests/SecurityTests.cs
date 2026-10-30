using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Symposium.Api.Server.Controllers;
using Symposium.Api.Server.Services;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Storage;
using Xunit;

namespace Symposium.Tests;

public class SecurityTests
{
    private static AppDbContext Database() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task UnconfiguredClientReturnsCompletedNullTask()
    {
        await using var db = Database();
        var service = new AuthenticatorService(db, new ConfigurationBuilder().Build());
        Assert.Null(await service.GetClient("unknown", null!));
    }

    [Fact]
    public async Task MemberReceivesOnlyMemberRole()
    {
        await using var db = Database();
        var service = new AuthenticatorService(db, new ConfigurationBuilder().Build());
        var claims = await service.MakeClaims(new Client(), new user { id = 1, type = UserType.MEMBER });
        Assert.Contains(claims.Claims, a => a.Type == ClaimTypes.Role && a.Value == "Member");
        Assert.DoesNotContain(claims.Claims, a => a.Type == ClaimTypes.Role && a.Value == "Admin");
    }

    [Fact]
    public async Task SelfUpdateCannotElevateRoleOrReplacePassword()
    {
        await using var db = Database();
        db.users.Add(new user { id = 1, email = "member@example.com", password = "original", type = UserType.MEMBER });
        await db.SaveChangesAsync();
        var controller = new UserController(db, null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("user_id", "1")], "test"))
                }
            }
        };
        await controller.update_user(new User
        {
            email = "member@example.com", type = UserType.ADMIN, password = "attacker-password"
        });
        var entity = await db.users.SingleAsync();
        Assert.Equal(UserType.MEMBER, entity.type);
        Assert.Equal("original", entity.password);
    }

    [Fact]
    public async Task RegistrationIsDisabledByDefault()
    {
        await using var db = Database();
        var controller = new AccountController(db, null!);
        Assert.IsType<NotFoundResult>(await controller.Register(new RegisterModel(), new ConfigurationBuilder().Build()));
        Assert.Empty(db.users);
    }

    [Fact]
    public void PaginationMappingPreservesTotal()
    {
        var page = new Pagination<int> { currentPage = 2, pageCount = 3, itemCount = 35, data = [1, 2] };
        Assert.Equal(35, page.Select(a => a.ToString()).itemCount);
    }

    [Theory]
    [InlineData("0", "15")]
    [InlineData("1", "0")]
    [InlineData("1", "-2")]
    [InlineData("1", "1001")]
    public async Task InvalidPaginationIsRejected(string page, string size)
    {
        var controller = new AccountController(null!, null!)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        controller.Request.QueryString = new QueryString($"?page={page}&itemPerPage={size}");
        await Assert.ThrowsAsync<ArgumentException>(() => controller.Paginate(new[] { 1 }.AsQueryable()));
    }

    [Fact]
    public void StartupFailsWithoutSecrets()
    {
        Assert.Throws<InvalidOperationException>(() => StartupConfiguration.Validate(new ConfigurationBuilder().Build()));
    }
}
